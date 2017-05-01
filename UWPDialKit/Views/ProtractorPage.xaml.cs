using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPDialKit
{
    public sealed partial class ProtractorPage : Page
    {
        private RadialController Controller;

        public ProtractorPage()
        {
            InitializeComponent();
            Loaded += ProtractorPage_Loaded;
        }

        private void ProtractorPage_Loaded(object sender, RoutedEventArgs e)
        {
            Controller = RadialController.CreateForCurrentView();
            CreateMenuItem();
            ProtractorControl.Controller = Controller;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (Controller != null)
            {
                Controller.Menu.Items.Clear();
            }
        }

        private void InitializeController()
        {
            Controller = RadialController.CreateForCurrentView();
            Controller.RotationResolutionInDegrees = 1;
        }

        private void CreateMenuItem()
        {
            RadialControllerMenuItem radialControllerItem = RadialControllerMenuItem.CreateFromKnownIcon("Protractor", RadialControllerMenuKnownIcon.Ruler);
            Controller.Menu.Items.Add(radialControllerItem);
            radialControllerItem.Invoked += ColorPalettePage_Invoked;
        }

        private void ColorPalettePage_Invoked(RadialControllerMenuItem sender, object args)
        {
            ProtractorControl.IsActive = true;
        }

    }
}
