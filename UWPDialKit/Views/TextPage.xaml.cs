using System;
using Windows.Storage.Streams;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPDialKit.Views
{
    public sealed partial class TextPage : Page
    {
        RadialController Controller;
        bool IsTextToolSelected;

        public TextPage()
        {
            InitializeComponent();
            Loaded += TextPage_Loaded;
        }

        private void TextPage_Loaded(object sender, RoutedEventArgs e)
        {
            SampleText.Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
            
            Controller = RadialController.CreateForCurrentView();
            CreateMenuItem();
            Controller.RotationChanged += Controller_RotationChanged;
        }

        private void Controller_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            if (IsTextToolSelected)
            {
                if (args.RotationDeltaInDegrees > 0 && SampleText.FontSize < 60)
                {
                    SampleText.FontSize++;
                }
                else if (args.RotationDeltaInDegrees < 0 && SampleText.FontSize > 7)
                {
                    SampleText.FontSize--;
                }
            }
        }

        private void CreateMenuItem()
        {
            RandomAccessStreamReference icon = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/text_size_icon.png"));
            RadialControllerMenuItem radialControllerItem = RadialControllerMenuItem.CreateFromIcon("Text Size", icon);
            Controller.Menu.Items.Add(radialControllerItem);
            radialControllerItem.Invoked += RadialControllerItem_Invoked;
        }

        private void RadialControllerItem_Invoked(RadialControllerMenuItem sender, object args)
        {
            IsTextToolSelected = true;
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
