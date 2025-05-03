using SRTPluginManager.Core;

namespace SRTPluginManager.MVVM.ViewModel
{
    class MainViewModel : ObserableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand TrainerViewCommand { get; set; }
        public RelayCommand PluginViewCommand { get; set; }
        public RelayCommand ExtensionViewCommand { get; set; }
        public RelayCommand WidgetViewCommand { get; set; }
        public RelayCommand InterfaceViewCommand { get; set; }
        public RelayCommand UpdatesViewCommand { get; set; }
        public RelayCommand ContactUsViewCommand { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public TrainerViewModel TrainerVM{ get; set; }
        public PluginViewModel PluginVM { get; set; }
        public ExtensionsViewModel ExtensionVM { get; set; }
        public WidgetViewModel WidgetVM { get; set; }
        public InterfaceViewModel InterfaceVM { get; set; }
        public UpdatesViewModel UpdatesVM { get; set; }
        public ContactUsViewModel ContactUsVM { get; set; }

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
            TrainerVM = new TrainerViewModel();
            PluginVM = new PluginViewModel();
            ExtensionVM = new ExtensionsViewModel();
            WidgetVM = new WidgetViewModel();
            InterfaceVM = new InterfaceViewModel();
            UpdatesVM = new UpdatesViewModel();
            ContactUsVM = new ContactUsViewModel();

            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o => 
            {
                CurrentView = HomeVM;
            });

            TrainerViewCommand = new RelayCommand(o =>
            {
                CurrentView = TrainerVM;
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

            InterfaceViewCommand = new RelayCommand(o =>
            {
                CurrentView = InterfaceVM;
            });

            UpdatesViewCommand = new RelayCommand(o =>
            {
                CurrentView = UpdatesVM;
            });

            ContactUsViewCommand = new RelayCommand(o =>
            {
                CurrentView = ContactUsVM;
            });
        }
    }
}
