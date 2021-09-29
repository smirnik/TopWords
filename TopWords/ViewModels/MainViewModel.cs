using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using System.Windows.Forms;
using Microsoft.Toolkit.Mvvm.Messaging;
using TopWords.Messages;

namespace TopWords.ViewModels
{
    class MainViewModel : ObservableObject
    {
        private readonly IMessenger _messenger;
        private string _path;
        private bool _isSearchInSubfolders;

        public MainViewModel(IMessenger messenger)
        {
            Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            SelectPathCommand = new RelayCommand(SelectPath);
            RunSearchCommand = new RelayCommand(RunSearch);
            _messenger = messenger;
        }

        public string Path
        {
            get => _path; 
            set => SetProperty(ref _path, value);
        }

        public ICommand SelectPathCommand
        {
            get; private set;
        }

        public ICommand RunSearchCommand
        {
            get; private set;
        }

        public bool IsSearchInSubfolders
        {
            get => _isSearchInSubfolders;
            set => SetProperty(ref _isSearchInSubfolders, value);
        }

        private void RunSearch()
        {
            var window = new Views.SearchWindow();
            _messenger.Send(new SearchCommandMessage(Path, IsSearchInSubfolders));
            window.ShowDialog();
        }

        private void SelectPath()
        {
            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                Description = "Select folder with text files",
                SelectedPath = Path,
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Path = dialog.SelectedPath;
            }
        }
    }
}
