using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleCompositionSample
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }


        public void AddStoryboard()
        {
            // setup composite render transform
            if (!(FancyPanel.RenderTransform is CompositeTransform))
            {
                FancyPanel.RenderTransform = new CompositeTransform();
                FancyPanel.RenderTransformOrigin = new Point(0.5, 0.5);
            }
            var sb = new Storyboard {Duration = new Duration(TimeSpan.FromSeconds(3))};
            var rotateAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = sb.Duration
            };

            Storyboard.SetTarget(rotateAnimation, FancyPanel);
            Storyboard.SetTargetProperty(rotateAnimation, 
                "(RelativePanel.RenderTransform).(CompositeTransform.Rotation)");

            sb.Children.Add(rotateAnimation);
            sb.Begin();
        }

        public void AddComposition()
        {
            var panelVisual = ElementCompositionPreview.GetElementVisual(FancyPanel);

            var compositor = panelVisual.Compositor;

            var rotateAnimation = compositor.CreateScalarKeyFrameAnimation();
            var easing = compositor.CreateLinearEasingFunction();
            rotateAnimation.InsertKeyFrame(0.0f, 0f);
            rotateAnimation.InsertKeyFrame(1.0f, 360f, easing);

            rotateAnimation.Duration = TimeSpan.FromSeconds(3);
            rotateAnimation.IterationBehavior = AnimationIterationBehavior.Count;
            rotateAnimation.IterationCount = 1;

            panelVisual.StartAnimation(nameof(panelVisual.RotationAngleInDegrees), rotateAnimation);
            panelVisual.CenterPoint = new Vector3((float)FancyPanel.ActualWidth / 2.0f, (float)FancyPanel.ActualHeight / 2.0f, 0);
        }
    }
}