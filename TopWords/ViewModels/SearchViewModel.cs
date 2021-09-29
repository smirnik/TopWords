using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TopWords.Messages;
using TopWords.Services.FrequencyAnalizer.ProgressReporting;
using TopWordsTestApp;

namespace TopWords.ViewModels
{
    public class SearchViewModel : ObservableObject, IRecipient<SearchCommandMessage>
    {
        private IFrequencyAnalizer _frequencyAnalizer;
        private readonly IMessenger _messenger;
        private readonly CancellationTokenSource _cancallationTokenSource;
        
        private readonly ObservableCollection<WordCount> _wordsList = new();
        private int _maxCount;
        private string _status;
        private string _details;
        private int _filesCount;
        private int _processedCount;
        private int _failedCount;
        private int _wordsCount;
        private bool _isRunning;
        private TaskStatus _taskStatus;
        private bool _showResult;

        public SearchViewModel(IFrequencyAnalizer frequencyAnalizer, IMessenger messenger)
        {
            _frequencyAnalizer = frequencyAnalizer;
            _messenger = messenger;
            _cancallationTokenSource = new CancellationTokenSource();
            CancelCommand = new RelayCommand(Cancel);

            messenger.RegisterAll(this);
        }

        public int FilesCount
        {
            get => _filesCount;
            set => SetProperty(ref _filesCount, value);
        }

        public int ProcessedCount
        {
            get => _processedCount;
            set => SetProperty(ref _processedCount, value);
        }

        public int FailedCount
        {
            get => _failedCount;
            set => SetProperty(ref _failedCount, value);
        }

        public int WordsCount
        {
            get => _wordsCount;
            set => SetProperty(ref _wordsCount, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }

        public int MaxCount
        {
            get => _maxCount;
            set => SetProperty(ref _maxCount, value);
        }

        private void Cancel()
        {
            _cancallationTokenSource?.Cancel();
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool ShowResult
        {
            get => _showResult;
            set => SetProperty(ref _showResult, value);
        }

        public ICommand CancelCommand { get; set; }

        public ObservableCollection<WordCount> WordsList => _wordsList;

        public event EventHandler<string> Log;

        private void OnLog(string entry)
        {
            Log?.Invoke(this, entry);
        }

        public async void RunSearch(string path, bool isSearchInSubfolders)
        {
            
            IsRunning = true;
            try
            {
                var progress = new Progress<object>(OnProgressReported);
                var words = await _frequencyAnalizer.GetTopWordsAsync(path, isSearchInSubfolders, 10, progress, _cancallationTokenSource.Token);
                if (words.Any())
                {
                    MaxCount = words.Max(o => o.Count);
                    words.ForEach(word => WordsList.Add(word));
                }
                TaskStatus = TaskStatus.RanToCompletion;
                ShowResult = true;
            }
            catch(OperationCanceledException ex)
            {
                OnLog($"Canceled");
                Status = "Canceled";
                Details = "";
                TaskStatus = TaskStatus.Canceled;
            }
            catch(Exception ex)
            {
                OnLog($"Error occured: {ex.Message}");
                Status = "Error";
                Details = "";
                TaskStatus = TaskStatus.Faulted;
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void OnProgressReported(object progress)
        {
            switch(progress)
            {
                case FoundFilesCount foundFilesCount:
                    FilesCount = foundFilesCount.FilesCount;
                    break;
                case StatusChanged statusChanged:
                    Status = statusChanged.Status;
                    Details = statusChanged.Details;
                    break;
                case FileProcessed fileProcessed:
                    OnLog($"Found {fileProcessed.WordsCountTotal} words in '{fileProcessed.FileName}'");
                    ProcessedCount++;
                    WordsCount = fileProcessed.WordsCountTotal;
                    break;
                case ErrorOccured error:
                    OnLog($"Error: {error.Error}");
                    FailedCount++;
                    break;
            }
        }

        public TaskStatus TaskStatus
        {
            get => _taskStatus;
            set => SetProperty(ref _taskStatus, value);
        }

        public void Receive(SearchCommandMessage message)
        {
            _messenger.Unregister<SearchCommandMessage>(this);
            RunSearch(message.Path, message.SearchInSubfolders);
        }
    }
}