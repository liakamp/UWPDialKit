using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPDialKit.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPDialKit
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {

        private List<NavMenuItem> navlist = new List<NavMenuItem>(
           new[]
           {
                new NavMenuItem()
                {
                    Image =  "ms-appx:///Assets/menu_palette.png",                    
                    Label = "Color Palette",
                    DestPage = typeof(ColorPalettePage)
                },
                new NavMenuItem()
                {
                    Image =  "ms-appx:///Assets/menu_protractor.png",
                    Label = "Protractor",
                    DestPage = typeof(ProtractorPage)
                }                           
           });


        public Shell()
        {
            InitializeComponent();
            NavMenuList.ItemsSource = navlist;
            NavMenuList.SelectedIndex = 0;
            AppFrame.Navigate(typeof(ColorPalettePage));            
        }

        private void NavMenuList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as NavMenuItem;

            if (item != null)
            {                
                if (item.DestPage != null &&
                    item.DestPage != AppFrame.CurrentSourcePageType)
                {
                    AppFrame.Navigate(item.DestPage);
                }
            }
        }
    }
}
