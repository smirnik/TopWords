using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TopWords.ViewModels;

namespace TopWordsTestApp
{
    public interface IFrequencyAnalizer
    {
        Task<List<WordCount>> GetTopWordsAsync(string path, bool searchInSubfolders, int count, IProgress<object> progress, CancellationToken cancellationToken);
    }
}