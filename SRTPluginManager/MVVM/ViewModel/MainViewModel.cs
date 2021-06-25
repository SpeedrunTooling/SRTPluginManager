using System;
using SRTPluginManager.Core;

namespace SRTPluginManager.MVVM.ViewModel
{
    class MainViewModel : ObserableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand PluginViewCommand { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public PluginViewModel PluginVM { get; set; }

        private object _currentView;
        public object CurrentView 
        { 
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            PluginVM = new PluginViewModel();
            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o => 
            {
                CurrentView = HomeVM;
            });

            PluginViewCommand = new RelayCommand(o => 
            {
                CurrentView = PluginVM;
            });
        }
    }
}
