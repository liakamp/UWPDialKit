using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPDialKit
{
    public sealed partial class ColorPalettePage : Page
    {
        private RadialController Controller;

        public ColorPalettePage()
        {
            InitializeComponent();
            Loaded += ColorPalettePage_Loaded;         
        }

        private void ColorPalettePage_Loaded(object sender, RoutedEventArgs e)
        {
            Controller = RadialController.CreateForCurrentView();
            CreateMenuItem();
            PaletteControl.Controller = Controller;
        }       

        private void CreateMenuItem()
        {
            RadialControllerMenuItem radialControllerItem = RadialControllerMenuItem.CreateFromKnownIcon("Palette", RadialControllerMenuKnownIcon.InkColor);
            Controller.Menu.Items.Add(radialControllerItem);
            radialControllerItem.Invoked += ColorPalettePage_Invoked;
        }

        private void ColorPalettePage_Invoked(RadialControllerMenuItem sender, object args)
        {
            PaletteControl.IsActive = true;
        }        

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {            
            if (Controller != null)
            {
                Controller.Menu.Items.Clear();
            }
        }

    }
}
