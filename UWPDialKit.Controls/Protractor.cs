using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPDialKit.Controls.Helpers;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace UWPDialKit.Controls
{
    [TemplatePart(Name = "ProtractorContainer", Type = typeof(Grid))]
    [TemplatePart(Name = "AngleRay", Type = typeof(Path))]    
    public sealed class Protractor : Control
    {
        Storyboard storyboard;
        bool isRotating;
        double currentAngle;

        private Grid protractorContainer;
        private Path angleRay;

        public static readonly DependencyProperty IsActiveProperty =
           DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(Protractor), new PropertyMetadata(false, IsActiveChangedCallback));

        private static void IsActiveChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var palette = d as Protractor;
            if (palette.IsActive)
            {
                palette.SetupController();
            }
            else
            {
                palette.SetupController();
            }
        }

        public static readonly DependencyProperty CurrentAngleProperty =
           DependencyProperty.Register(nameof(CurrentAngle), typeof(double), typeof(Protractor), new PropertyMetadata(0));

        public bool IsActive
        {
            get
            {
                return (bool)GetValue(IsActiveProperty);
            }
            set
            {
                SetValue(IsActiveProperty, value);
            }
        }

        public double CurrentAngle
        {
            get
            {
                return (double)GetValue(CurrentAngleProperty);
            }
            set
            {
                SetValue(CurrentAngleProperty, value);
            }
        }

        public RadialController Controller { get; set; }


        public Protractor()
        {
            DefaultStyleKey = typeof(Protractor);
        }

        protected override void OnApplyTemplate()
        {
            protractorContainer = GetTemplateChild("ProtractorContainer") as Grid;            
            angleRay = GetTemplateChild("AngleRay") as Path;            

            storyboard = new Storyboard();
            storyboard.Completed += Storyboard_Completed;
            currentAngle = 0;
            SizeChanged += MainPage_SizeChanged;
           
            base.OnApplyTemplate();
        }

        private void SetupController()
        {
            if (Controller == null) Controller = RadialController.CreateForCurrentView();
            Controller.RotationResolutionInDegrees = 1;
            Controller.RotationChanged += Controller_RotationChanged;            
        }       

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            angleRay.Data = new LineGeometry() { StartPoint = new Point(0, 0), EndPoint = new Point(protractorContainer.ActualWidth / 2, 0) };
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

            currentAngle += angle;

            DoubleAnimation rayAnimation = new DoubleAnimation();                  
            CurrentAngle = Utilities.ClampAngle(currentAngle);
            rayAnimation.To = currentAngle;

            rayAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            storyboard.Children.Add(rayAnimation);
            Storyboard.SetTarget(rayAnimation, angleRay);
            Storyboard.SetTargetProperty(rayAnimation, "(AngleRay.RenderTransform).(RotateTransform.Angle)");
            storyboard.Begin();
            isRotating = true;
        }

        private void CompleteRotation()
        {
            storyboard.SkipToFill();
            storyboard.Stop();
            RotateTransform rotateTransform = new RotateTransform() { Angle = currentAngle };
            angleRay.RenderTransform = rotateTransform;
            storyboard.Children.Clear();
        }       

        private void Controller_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            if (IsActive)
            {
                Rotate(args.RotationDeltaInDegrees);
            }
        }

    }
}
