using System;
using System.Collections.Generic;
using UWPDialKit.Helpers;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace UWPDialKit
{   
    public sealed partial class ColorPalettePage : Page
    {
        private RadialController Controller;
        Storyboard storyboard;
        bool isRotating;
        double currentAngle;
        public ColorPalettePage()
        {
            InitializeComponent();
            storyboard = new Storyboard();
            storyboard.Completed += Storyboard_Completed;
            currentAngle = 0;
            ColorRay.Data = new LineGeometry() { StartPoint = new Point(175, 175), EndPoint = new Point(350, 175) };
            ColorEllipse.Data = new EllipseGeometry() { Center = new Point(338, 175), RadiusX = 10, RadiusY = 10 };
            InitializeController();
            CreateMenuItem();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (Controller != null)
            {
                Controller.Menu.Items.Clear();
            }
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Grid PaletteRing = new Grid();
            PaletteRing.Width = 300;
            PaletteRing.Height = 300;
            Color current = Utilities.ConvertHSV2RGB(0,1,1);
            for (int i = 0; i < 360; i++)
            {
                RotateTransform rt = new RotateTransform() { Angle = i };
                LinearGradientBrush colorBrush = new LinearGradientBrush();
                colorBrush.GradientStops.Add(new GradientStop() { Color = current });
                current = Utilities.ConvertHSV2RGB(i,1,1);
                colorBrush.GradientStops.Add(new GradientStop() { Color = current, Offset = 1 });
                Path line = new Path()
                {
                    Data = new LineGeometry() { StartPoint = new Point(280, 150), EndPoint = new Point(300, 150) },
                    StrokeThickness = 3,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = rt,
                    Stroke = colorBrush
                };
                PaletteRing.Children.Add(line);
            }
            PaletteRing.HorizontalAlignment = HorizontalAlignment.Center;
            PaletteRing.VerticalAlignment = VerticalAlignment.Center;
            LayoutRoot.Children.Add(PaletteRing);
        }

        private void InitializeController()
        {
            Controller = RadialController.CreateForCurrentView();
            Controller.RotationResolutionInDegrees = 1;            
            Controller.RotationChanged += Controller_RotationChanged;
            Controller.ButtonClicked += Controller_ButtonClicked;         
        }

        private void CreateMenuItem()
        {
            RadialControllerMenuItem radialControllerItem = RadialControllerMenuItem.CreateFromKnownIcon("Palette", RadialControllerMenuKnownIcon.InkColor);
            Controller.Menu.Items.Add(radialControllerItem);
        }            

        private void Controller_ButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            SelectedColor.Background = new SolidColorBrush(Utilities.ConvertHSV2RGB((float)Utilities.ClampAngle(currentAngle),1,1));
        }

        private void Controller_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {           
            Rotate(args.RotationDeltaInDegrees);
        }       

        private void Storyboard_Completed(object sender, object e)
        {
            isRotating = false;
            CompleteRotation();
        }       

        private void Rotate(double angle = 1)
        {
            if (isRotating)
            {
                CompleteRotation();
            }

            DoubleAnimation rayAnimation = new DoubleAnimation();
            currentAngle += angle;
            rayAnimation.To = currentAngle;
            rayAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            DoubleAnimation colorEllipseAnimation = new DoubleAnimation();
            colorEllipseAnimation.To = currentAngle;
            colorEllipseAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            storyboard.Children.Add(rayAnimation);
            Storyboard.SetTarget(rayAnimation, ColorRay);
            Storyboard.SetTargetProperty(rayAnimation, "(ColorRay.RenderTransform).(RotateTransform.Angle)");

            storyboard.Children.Add(colorEllipseAnimation);
            Storyboard.SetTarget(colorEllipseAnimation, ColorEllipse);
            Storyboard.SetTargetProperty(colorEllipseAnimation, "(ColorEllipse.RenderTransform).(RotateTransform.Angle)");

            storyboard.Begin();
            isRotating = true;
            Color activeColor = Utilities.ConvertHSV2RGB((float)Utilities.ClampAngle(currentAngle),1,1);
            ColorEllipse.Fill = CurrentColor.Background = 
                new SolidColorBrush(activeColor);
        }

        private void CompleteRotation()
        {
            storyboard.SkipToFill();
            storyboard.Stop();
            RotateTransform rotateTransform = new RotateTransform() { Angle = currentAngle };
            ColorRay.RenderTransform = rotateTransform;
            ColorEllipse.RenderTransform = rotateTransform;
            storyboard.Children.Clear();
        }        

        
                      
    }
}
