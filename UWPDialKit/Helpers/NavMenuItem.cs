using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPDialKit.Helpers
{   
   public class NavMenuItem
    {
        public string Label { get; set; }
        public string Image { get; set; }
        public Type DestPage { get; set; }   

    }
}
