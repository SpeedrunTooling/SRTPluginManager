using SRTPluginManager.Core;

namespace SRTPluginManager.MVVM.ViewModel
{
    class MainViewModel : ObserableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand HostViewCommand { get; set; }
        public RelayCommand PluginViewCommand { get; set; }
        public RelayCommand ExtensionViewCommand { get; set; }
        public RelayCommand WidgetViewCommand { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public HostViewModel HostVM { get; set; }
        public PluginViewModel PluginVM { get; set; }
        public ExtensionsViewModel ExtensionVM { get; set; }
        public WidgetViewModel WidgetVM { get; set; }

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
            HostVM = new HostViewModel();
            PluginVM = new PluginViewModel();
            ExtensionVM = new ExtensionsViewModel();
            WidgetVM = new WidgetViewModel();
            
            CurrentView = PluginVM;

            HomeViewCommand = new RelayCommand(o => 
            {
                CurrentView = HomeVM;
            });

            HostViewCommand = new RelayCommand(o => 
            {
                CurrentView = HostVM;
            });

            PluginViewCommand = new RelayCommand(o => 
            {
                CurrentView = PluginVM;
            });

            ExtensionViewCommand = new RelayCommand(o =>
            {
                CurrentView = ExtensionVM;
            });

            WidgetViewCommand = new RelayCommand(o =>
            {
                CurrentView = WidgetVM;
            });
        }
    }
}
