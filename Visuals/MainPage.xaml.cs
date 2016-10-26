using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Visuals
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

        private void AnimateVisual(object sender, RoutedEventArgs e)
        {
            var visual = ElementCompositionPreview.GetElementVisual(FancyPanel);
            var compositor = visual.Compositor;

            var translateAnimation = compositor.CreateVector3KeyFrameAnimation();
            var easing = compositor.CreateLinearEasingFunction();
            translateAnimation.InsertKeyFrame(0.0f, Vector3.Zero);
            translateAnimation.InsertKeyFrame(1.0f, new Vector3(600f, 400f, 0), easing);

            translateAnimation.Duration = TimeSpan.FromSeconds(2);
            translateAnimation.IterationBehavior = AnimationIterationBehavior.Count;
            translateAnimation.IterationCount = 1;

            visual.StartAnimation(nameof(visual.Offset), translateAnimation);

        }

        private void AddVisual(object sender, RoutedEventArgs e)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var visual = compositor.CreateSpriteVisual();

            visual.Size = new Vector2(150, 150);
            visual.Offset = new Vector3(50, 50, 0);
            visual.Brush = compositor.CreateColorBrush(Colors.Red);

            ElementCompositionPreview.SetElementChildVisual(FancyPanel, visual);
        }

        private void AddMany(object sender, RoutedEventArgs e)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var container = compositor.CreateContainerVisual();

            var visual1 = compositor.CreateSpriteVisual();

            visual1.Size = new Vector2(150, 150);
            visual1.Offset = new Vector3(50, 50, 0);
            visual1.Brush = compositor.CreateColorBrush(Colors.Red);

            var visual2 = compositor.CreateSpriteVisual();

            visual2.Size = new Vector2(150, 150);
            visual2.Offset = new Vector3(50, 260, 0);
            visual2.Brush = compositor.CreateColorBrush(Colors.Blue);

            container.Children.InsertAtTop(visual1);
            container.Children.InsertAtTop(visual2);

            ElementCompositionPreview.SetElementChildVisual(FancyPanel, container);
        }
    }
}