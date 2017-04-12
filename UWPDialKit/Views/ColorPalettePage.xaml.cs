using System;
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
        public ColorSelectionMode SelectionMode;
        float mainColorAngle;
        float activeSaturationValue;
        Grid PaletteRing;
        Grid SaturationPaletteRing;
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
            SelectionMode = ColorSelectionMode.Value;
            PaletteRing = new Grid();
            PaletteRing.Width = 300;
            PaletteRing.Height = 300;
            Color current = Utilities.ConvertHSV2RGB(0, 1, 1);
            for (int i = 0; i < 360; i++)
            {
                RotateTransform rt = new RotateTransform() { Angle = i };
                LinearGradientBrush colorBrush = new LinearGradientBrush();
                colorBrush.GradientStops.Add(new GradientStop() { Color = current });
                current = Utilities.ConvertHSV2RGB(i, 1, 1);
                colorBrush.GradientStops.Add(new GradientStop() { Color = current, Offset = 1 });
                Path line = DrawColorLine(rt, colorBrush);
                PaletteRing.Children.Add(line);
            }
            PaletteRing.HorizontalAlignment = HorizontalAlignment.Center;
            PaletteRing.VerticalAlignment = VerticalAlignment.Center;
            LayoutRoot.Children.Add(PaletteRing);
            SaturationPaletteRing = new Grid()
            {
                Width = 300,
                Height = 300,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            LayoutRoot.Children.Add(SaturationPaletteRing);
        }

        private static Path DrawColorLine(RotateTransform rt, LinearGradientBrush colorBrush)
        {
            return new Path()
            {
                Data = new LineGeometry() { StartPoint = new Point(280, 150), EndPoint = new Point(300, 150) },
                StrokeThickness = 3,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = rt,
                Stroke = colorBrush
            };
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
            if (SelectionMode == ColorSelectionMode.Value)
            {
                mainColorAngle = (float)Utilities.ClampAngle(currentAngle);
                SelectedColorBorder.Background = new SolidColorBrush(Utilities.ConvertHSV2RGB(mainColorAngle, 1, 1));
                SelectionMode = ColorSelectionMode.Saturation;
            }
            else
            {
                SelectedColorBorder.Background = new SolidColorBrush(Utilities.ConvertHSV2RGB(mainColorAngle, 1 - Math.Abs(activeSaturationValue), 1));
                SelectionMode = ColorSelectionMode.Value;
                activeSaturationValue = 0;
            }
            FillSelectionRing();

        }

        private void FillSelectionRing()
        {
            if (SelectionMode == ColorSelectionMode.Value)
            {
                PaletteRing.Visibility = Visibility.Visible;
                SaturationPaletteRing.Visibility = Visibility.Collapsed;
            }
            else
            {
                PaletteRing.Visibility = Visibility.Collapsed;
                FillSaturationRing();
                SaturationPaletteRing.Visibility = Visibility.Visible;

            }
        }

        private void FillSaturationRing()
        {
            SaturationPaletteRing.Children.Clear();
            Color current = Utilities.ConvertHSV2RGB(mainColorAngle, 0, 1);
            double rotationAngle = Utilities.ClampAngle(mainColorAngle - 100);
            for (int i = 0; i < 100; i++)
            {
                RotateTransform rt = new RotateTransform() { Angle = Utilities.ClampAngle(rotationAngle) };
                rotationAngle++;
                LinearGradientBrush colorBrush = new LinearGradientBrush();
                colorBrush.GradientStops.Add(new GradientStop() { Color = current });
                current = Utilities.ConvertHSV2RGB(mainColorAngle, i * 0.01f, 1);
                colorBrush.GradientStops.Add(new GradientStop() { Color = current, Offset = 1 });
                Path line = DrawColorLine(rt, colorBrush);
                SaturationPaletteRing.Children.Add(line);
            }
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

            if (SelectionMode == ColorSelectionMode.Saturation)
            {
                double destinationAngle = Utilities.ClampAngle(currentAngle + angle);

                if (!IsInSaturationBounds(destinationAngle))
                {
                    return;
                }
                activeSaturationValue += (float)angle / 100;
            }

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

            Color activeColor;
            if (SelectionMode == ColorSelectionMode.Value)
            {
                activeColor = Utilities.ConvertHSV2RGB((float)Utilities.ClampAngle(currentAngle), 1, 1);
            }
            else
            {
                activeColor = Utilities.ConvertHSV2RGB(mainColorAngle, 1 - Math.Abs(activeSaturationValue), 1);
            }

            ColorEllipse.Fill = CurrentColorBorder.Background = new SolidColorBrush(activeColor);
        }

        private bool IsInSaturationBounds(double destinationAngle)
        {
            if (mainColorAngle >= 100)
            {
                return (destinationAngle > mainColorAngle ||
                    destinationAngle < Utilities.ClampAngle(mainColorAngle - 100)) ? false : true;
            }
            else
            {
                return (destinationAngle < mainColorAngle ||
                    (destinationAngle > mainColorAngle && destinationAngle > Utilities.ClampAngle(mainColorAngle - 100))) ? true : false;
            }
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

        public enum ColorSelectionMode { Value, Saturation }

    }
}
