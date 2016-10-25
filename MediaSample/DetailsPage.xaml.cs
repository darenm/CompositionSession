using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;
using MediaSample.Annotations;
using MediaSample.Services;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MediaSample
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailsPage : Page, INotifyPropertyChanged
    {
        private readonly Compositor _compositor;
        private CompositionEffectBrush _brush;
        private Poster _selectedPoster;

        public DetailsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            #region #DEMO2# Blur Background

            //BlurBackground();

            #endregion
        }

        public Poster SelectedPoster
        {
            get { return _selectedPoster; }
            set
            {
                _selectedPoster = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SelectedPoster = e.Parameter as Poster;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            #region #DEMO5# forward animation

            //var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("HeroImage");
            //if (animation != null)
            //{
            //    HeroImage.Opacity = 0;
            //    HeroImage.ImageOpened += (sender, args) =>
            //    {
            //        animation.TryStart(HeroImage);
            //        HeroImage.Opacity = 1;
            //    };
            //}

            #endregion

            base.OnNavigatedTo(e);
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs backRequestedEventArgs)
        {
            #region #DEMO6# back navigation

            //ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackHeroImage", HeroImage);

            #endregion

            // Navigation
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            Frame.GoBack();
        }

        #region #DEMO2# BlurBackground

        //private void BlurBackground()
        //{
        //    var blendmode = BlendEffectMode.SoftLight;

        //    // Create a chained effect graph using a BlendEffect, blending color and blur
        //    var graphicsEffect = new BlendEffect
        //    {
        //        Mode = blendmode,
        //        Background = new ColorSourceEffect
        //        {
        //            Name = "Tint",
        //            Color = Colors.White
        //        },
        //        Foreground = new GaussianBlurEffect
        //        {
        //            Name = "Blur",
        //            Source = new CompositionEffectSourceParameter("Backdrop"),
        //            BlurAmount = 35.0f,
        //            BorderMode = EffectBorderMode.Hard
        //        }
        //    };

        //    var blurEffectFactory = _compositor.CreateEffectFactory(graphicsEffect,
        //        new[] {"Blur.BlurAmount", "Tint.Color"});

        //    // Create EffectBrush, BackdropBrush and SpriteVisual
        //    _brush = blurEffectFactory.CreateBrush();

        //    var destinationBrush = _compositor.CreateBackdropBrush();
        //    _brush.SetSourceParameter("Backdrop", destinationBrush);

        //    var blurSprite = _compositor.CreateSpriteVisual();
        //    blurSprite.Size = new Vector2(
        //        (float) BackgroundImage.ActualWidth,
        //        (float) BackgroundImage.ActualHeight);
        //    blurSprite.Brush = _brush;

        //    ElementCompositionPreview.SetElementChildVisual(BackgroundImage, blurSprite);
        //}

        //private void BackgroundImage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    var blurVisual = (SpriteVisual) ElementCompositionPreview.GetElementChildVisual(BackgroundImage);

        //    if (blurVisual != null)
        //    {
        //        blurVisual.Size = e.NewSize.ToVector2();
        //    }
        //}

        #endregion
    }
}