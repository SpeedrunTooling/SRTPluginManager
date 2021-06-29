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
        public RelayCommand LogsViewCommand { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public HostViewModel HostVM { get; set; }
        public PluginViewModel PluginVM { get; set; }
        public ExtensionsViewModel ExtensionVM { get; set; }
        public WidgetViewModel WidgetVM { get; set; }
        public LogsViewModel LogsVM { get; set; }

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
            LogsVM = new LogsViewModel();
            
            CurrentView = HomeVM;

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

            LogsViewCommand = new RelayCommand(o =>
            {
                CurrentView = LogsVM;
            });
        }
    }
}
