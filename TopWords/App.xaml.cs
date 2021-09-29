using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Windows;
using TopWords.Services;
using TopWords.ViewModels;
using TopWordsTestApp;

namespace TopWords
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<IMessenger>((ServiceProvider) => WeakReferenceMessenger.Default);
            services.AddTransient<IFileFinder, FileFinder>();
            services.AddTransient<IFrequencyAnalizer, FrequencyAnalizer>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<SearchViewModel>();

            return services.BuildServiceProvider();
        }
    }


}
