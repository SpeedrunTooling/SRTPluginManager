using SRTPluginManager.Core;

namespace SRTPluginManager.MVVM.ViewModel
{
    class MainViewModel : ObserableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand PluginViewCommand { get; set; }
        public RelayCommand ExtensionViewCommand { get; set; }
        public RelayCommand WidgetViewCommand { get; set; }
        public HomeViewModel HomeVM { get; set; }
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
            PluginVM = new PluginViewModel();
            ExtensionVM = new ExtensionsViewModel();
            WidgetVM = new WidgetViewModel();
            
            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o => 
            {
                CurrentView = HomeVM;
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
