using System;
using UWPDialKit.Helpers;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace UWPDialKit
{    
    public sealed partial class ProtractorPage : Page
    {
        private RadialController Controller;
        Storyboard storyboard;
        bool isRotating;
        double currentAngle;
        public ProtractorPage()
        {
            InitializeComponent();
            storyboard = new Storyboard();
            storyboard.Completed += Storyboard_Completed;
            currentAngle = 0;
            SizeChanged += MainPage_SizeChanged;
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

        private void InitializeController()
        {
            Controller = RadialController.CreateForCurrentView();
            Controller.RotationResolutionInDegrees = 1;
            Controller.RotationChanged += Controller_RotationChanged;            
        }

        private void CreateMenuItem()
        {
            RadialControllerMenuItem radialControllerItem = RadialControllerMenuItem.CreateFromKnownIcon("Protractor", RadialControllerMenuKnownIcon.Ruler);
            Controller.Menu.Items.Add(radialControllerItem);
        }        

        private void Controller_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            Rotate(args.RotationDeltaInDegrees);
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AngleRay.Data = new LineGeometry() { StartPoint = new Point(0, 0), EndPoint = new Point(ProtractorContainer.ActualWidth / 2, 0) };
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
            AngleTxt.Text = "Angle: " + Utilities.ClampAngle(currentAngle).ToString() + " degrees";
            rayAnimation.To = currentAngle;

            rayAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            storyboard.Children.Add(rayAnimation);
            Storyboard.SetTarget(rayAnimation, AngleRay);
            Storyboard.SetTargetProperty(rayAnimation, "(AngleRay.RenderTransform).(RotateTransform.Angle)");
            storyboard.Begin();
            isRotating = true;
        }

        private void CompleteRotation()
        {
            storyboard.SkipToFill();
            storyboard.Stop();
            RotateTransform rotateTransform = new RotateTransform() { Angle = currentAngle };
            AngleRay.RenderTransform = rotateTransform;
            storyboard.Children.Clear();
        }        

    }
}
