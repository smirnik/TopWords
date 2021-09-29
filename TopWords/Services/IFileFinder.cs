using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TopWords.Services
{
    public interface IFileFinder
    {
        Task<List<string>> FindFilesAsync(string path, string searchPattern, bool searchInSubfolders, IProgress<object> progress, CancellationToken cancellationToken);
    }
}