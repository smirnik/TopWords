using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TopWords.Services;
using TopWords.ViewModels;
using TopWords.Services.FrequencyAnalizer.ProgressReporting;

namespace TopWordsTestApp
{
    public class FrequencyAnalizer : IFrequencyAnalizer
    {
        private readonly IFileFinder _fileFinder;
        private SpinLock fileSpinLock = new();
        public FrequencyAnalizer(IFileFinder fileFinder)
        {
            _fileFinder = fileFinder;
        }

        public async Task<List<WordCount>> GetTopWordsAsync(string path, bool searchInSubfolders, int count, IProgress<object> progress, CancellationToken cancellationToken)
        {
            var dictionary = new ConcurrentDictionary<string, int>();

            progress.Report(SearchProgress.Status("Searching files"));
            var files = await _fileFinder.FindFilesAsync(path, "*.txt", searchInSubfolders, progress, cancellationToken).ConfigureAwait(false);
            progress.Report(SearchProgress.FilesCount(files.Count));

            progress.Report(SearchProgress.Status("Countinging words"));
            
            Parallel.ForEach(files, (file, state) =>
            {
                Dictionary<string, int> wordsInFile = null;
                try
                {
                    wordsInFile = CountWordsInFile(file);
                    foreach (var word in wordsInFile)
                    {
                        dictionary.AddOrUpdate(word.Key, word.Value, (key, value) => value + word.Value);
                    }
                }
                catch(Exception ex)
                {
                    progress.Report(SearchProgress.Error($"{file}: {ex.Message}"));
                }
                
                progress.Report(SearchProgress.FileProcessed(file, wordsInFile == null ? 0 : wordsInFile.Count, dictionary.Count));

                if (cancellationToken.IsCancellationRequested)
                {
                    state.Stop();
                }
            });

            cancellationToken.ThrowIfCancellationRequested();
            
            progress.Report(SearchProgress.Status("Search completed"));
            return dictionary.OrderByDescending(word => word.Value).Take(count).Select(word => new WordCount(word.Key, word.Value)).ToList();
        }

        private Dictionary<string, int> CountWordsInFile(string fileName)
        {
            //Limit file reading threads to 1
            bool gotLock = false;
            string fileContent;
            try
            {
                fileSpinLock.TryEnter(ref gotLock);
                fileContent = File.ReadAllText(fileName, Encoding.Default);
            }
            finally
            {
                if (gotLock)
                {
                    fileSpinLock.Exit();
                }
            }

            return fileContent
                .Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.ToLower())
                .GroupBy(word => word)
                .ToDictionary(group => group.Key, group => group.Count());
        }
    }
}