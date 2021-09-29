using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopWords.Services.FrequencyAnalizer.ProgressReporting;

namespace TopWords.Services
{
    class FileFinder : IFileFinder
    {
        EnumerationOptions _enumerationOptions = new() { IgnoreInaccessible = true };

        public Task<List<string>> FindFilesAsync(string path, string searchPattern, bool searchInSubfolders, IProgress<object> progress, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var dir = new DirectoryInfo(path);
                return FindFiles(dir, searchPattern, searchInSubfolders, progress, cancellationToken);
            });
        }

        private List<string> FindFiles(DirectoryInfo dir, string searchPattern, bool searchInSubfolders, IProgress<object> progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress.Report(SearchProgress.Status("Searching files", dir.FullName));
            var files = dir.GetFiles(searchPattern, _enumerationOptions).Select(file => file.FullName).ToList();
            if (searchInSubfolders)
            {
                foreach (var sudDir in dir.GetDirectories("*", _enumerationOptions))
                {
                    files.AddRange(FindFiles(sudDir, searchPattern, searchInSubfolders, progress, cancellationToken));
                }
            }

            
            return files;
        }
    }
}
