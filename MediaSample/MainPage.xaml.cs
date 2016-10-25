using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MediaSample.Annotations;
using MediaSample.Services;
using Microsoft.Graphics.Canvas.Effects;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaSample
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private readonly Compositor _compositor;
        private CompositionEffectBrush _brush;
        private ObservableCollection<Poster> _posters;
        private Poster _selectedPoster;

        public MainPage()
        {
            InitializeComponent();
            // This keeps the main page in memory so I don't need to reload the collection
            // and reset the state
            NavigationCacheMode = NavigationCacheMode.Required;

            // The compositor is essential for using the Composition engine
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            #region #DEMO2# Blur Background

            //// As we are using a BackDrop brush we can set this up once
            //BackgroundImage.SizeChanged += BackgroundImage_OnSizeChanged;
            //BlurBackground();

            #endregion

            #region #DEMO3# DropShadow

            // As we adapt to the size of the selected image, we can do this once
            //HeroImage.SizeChanged += HeroImage_OnSizeChanged;
            //InitializeDropShadow(ShadowHost);

            #endregion
        }

        /// <summary>
        ///     The list of <see cref="Poster" /> objects we list.
        /// </summary>
        public ObservableCollection<Poster> Posters
        {
            get { return _posters; }
            set
            {
                _posters = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     The currently selected <see cref="Poster" />
        /// </summary>
        public Poster SelectedPoster
        {
            get { return _selectedPoster; }
            set
            {
                // This guard prevents a recursive loop where changing the selection on the 
                // ListView sets this value, which then raises a PropertyChanged... that resets
                // the SelectedItem on the ListView... which then calls this again...
                // This is caused by TwoWay Binding.
                if (_selectedPoster == value)
                {
                    return;
                }
                _selectedPoster = value;
                OnPropertyChanged();
                #region #DEMO4#
                //FadeInDetails();
                #endregion
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            #region #DEMO6#
            //ShadowHost.Visibility = Visibility.Collapsed;
            #endregion

            if (e.NavigationMode == NavigationMode.Back)
            {
                #region #DEMO6# Back Connected Animation

                //var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackHeroImage");
                //if (animation != null)
                //{
                //    animation.Completed += (sender, args) => { ShadowHost.Visibility = Visibility.Visible; };
                //    animation.TryStart(HeroImage);
                //}

                #endregion
            }
            else
            {
                Posters = new ObservableCollection<Poster>();
                var posterService = new PosterService();
                foreach (var poster in await posterService.GetPostersAsync())
                    Posters.Add(poster);
                PostersList.SelectedIndex = 0;
                PostersList.Focus(FocusState.Programmatic);
                #region #DEMO3
                //HeroImage.ImageOpened += HeroImage_ImageOpened;
                #endregion
            }
        }

        public void OpenDetail()
        {
            #region #DEMO5# Forward Connected Animation

            //ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("HeroImage", HeroImage);

            #endregion

            Frame.Navigate(typeof(DetailsPage), SelectedPoster);
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
        //            Source = new CompositionEffectSourceParameter("Backdrop"), // Source will be input
        //            BlurAmount = 35.0f,
        //            BorderMode = EffectBorderMode.Hard // prevents blur exceeding input bounds
        //        }
        //    };

        //    var blurEffectFactory = _compositor.CreateEffectFactory(graphicsEffect);

        //    // Create EffectBrush, BackdropBrush and SpriteVisual
        //    _brush = blurEffectFactory.CreateBrush();

        //    var destinationBrush = _compositor.CreateBackdropBrush();

        //    // sets the Source input of GuassianBlur
        //    _brush.SetSourceParameter("Backdrop", destinationBrush);

        //    var blurSprite = _compositor.CreateSpriteVisual();
        //    blurSprite.Size = new Vector2(
        //        (float)BackgroundImage.ActualWidth,
        //        (float)BackgroundImage.ActualHeight);
        //    blurSprite.Brush = _brush;

        //    ElementCompositionPreview.SetElementChildVisual(BackgroundImage, blurSprite);
        //}

        //private void BackgroundImage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    var blurVisual = (SpriteVisual)ElementCompositionPreview.GetElementChildVisual(BackgroundImage);

        //    if (blurVisual != null)
        //    {
        //        blurVisual.Size = e.NewSize.ToVector2();
        //    }
        //}

        #endregion

        #region #DEMO3# DropShadow

        //private void InitializeDropShadow(UIElement shadowHost)
        //{
        //    // This gets the actual visual for the shadowHost
        //    var hostVisual = ElementCompositionPreview.GetElementVisual(shadowHost);

        //    // Create a drop shadow
        //    var dropShadow = _compositor.CreateDropShadow();
        //    dropShadow.Color = Color.FromArgb(255, 75, 75, 80);
        //    dropShadow.BlurRadius = 15.0f;
        //    dropShadow.Offset = new Vector3(2.5f, 2.5f, 0.0f);

        //    // Create a Visual to hold the shadow
        //    var shadowVisual = _compositor.CreateSpriteVisual();
        //    shadowVisual.Shadow = dropShadow;

        //    // Add the shadow as a child of the host in the visual tree
        //    ElementCompositionPreview.SetElementChildVisual(shadowHost, shadowVisual);

        //    // Make sure size of shadow host and shadow visual always stay in sync
        //    var bindSizeAnimation = _compositor.CreateExpressionAnimation("hostVisual.Size");
        //    bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);

        //    shadowVisual.StartAnimation("Size", bindSizeAnimation);
        //}

        //private void HeroImage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    ShadowHost.Width = e.NewSize.Width;
        //}

        //// Called once on initial load
        //private void HeroImage_ImageOpened(object sender, RoutedEventArgs e)
        //{
        //    ShadowHost.Visibility = Visibility.Visible;
        //    HeroImage.ImageOpened -= HeroImage_ImageOpened;
        //}

        #endregion

        #region #DEMO4# FadeInDetails

        //private void FadeInDetails()
        //{
        //    var detailsVisual = ElementCompositionPreview.GetElementVisual(DetailGrid);
        //    var opacityAnimation = _compositor.CreateScalarKeyFrameAnimation();

        //    var easing = _compositor.CreateLinearEasingFunction();
        //    opacityAnimation.InsertKeyFrame(0.0f, 0f);
        //    opacityAnimation.InsertKeyFrame(1.0f, 1.0f, easing);

        //    opacityAnimation.Duration = TimeSpan.FromMilliseconds(500);
        //    opacityAnimation.IterationBehavior = AnimationIterationBehavior.Count;
        //    opacityAnimation.IterationCount = 1;

        //    detailsVisual.StartAnimation(nameof(detailsVisual.Opacity), opacityAnimation);
        //}

        #endregion

    }
}