using System;
using System.Windows;
using TopWords.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace TopWords.Views
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
            var vm = App.Current.Services.GetService<SearchViewModel>();
            DataContext = vm;
            vm.Log += ViewModelOnLog;
        }

        private void ViewModelOnLog(object sender, string logEntry)
        {
            logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss.ff}] {logEntry}{Environment.NewLine}");
        }
    }
}
