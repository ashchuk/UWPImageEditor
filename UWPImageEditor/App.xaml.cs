using System.Threading.Tasks;
using Prism.Unity.Windows;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;

namespace UWPImageEditor
{
    sealed partial class App : PrismUnityApplication
    {
        public App()
        {
            InitializeComponent();
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate("UWPImageEditor", null);
            return Task.FromResult<object>(null);
        }

        protected override Task OnActivateApplicationAsync(IActivatedEventArgs args)
        {
            NavigationService.Navigate("UWPImageEditor", null);
            return Task.FromResult<object>(null);
        }
    }
}
