using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
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
        private CompositionEffectBrush _brush;
        private Compositor _compositor;
        private ObservableCollection<Poster> _posters;
        private Poster _selectedPoster;
        private bool _showDetailsPane;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            BackgroundImage.Loaded += BackgroundImage_Loaded;
        }

        public ObservableCollection<Poster> Posters
        {
            get { return _posters; }
            set
            {
                _posters = value;
                OnPropertyChanged();
            }
        }

        public Poster SelectedPoster
        {
            get { return _selectedPoster; }
            set
            {
                if (_selectedPoster == value)
                {
                    return;
                }
                _selectedPoster = value;
                ShowDetailsPane = _selectedPoster != null;
                OnPropertyChanged();
            }
        }

        public bool ShowDetailsPane
        {
            get { return _showDetailsPane; }
            set
            {
                _showDetailsPane = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void BackgroundImage_Loaded(object sender, RoutedEventArgs e)
        {
            BlurBackground();
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                Posters = new ObservableCollection<Poster>();
                var posterService = new PosterService();
                foreach (var poster in await posterService.GetPostersAsync())
                {
                    Posters.Add(poster);
                }
                PostersList.SelectedIndex = 0;
                PostersList.Focus(FocusState.Programmatic);
            }

            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackHeroImage");
            if (animation != null)
            {
                animation.TryStart(HeroImage);
            }

        }

        public void OpenDetail()
        {
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("HeroImage", HeroImage);
            Frame.Navigate(typeof(DetailsPage), SelectedPoster);
        }

        #region BlurBackground

        private void BlurBackground()
        {
            var blendmode = BlendEffectMode.SoftLight;

            // Create a chained effect graph using a BlendEffect, blending color and blur
            var graphicsEffect = new BlendEffect
            {
                Mode = blendmode,
                Background = new ColorSourceEffect
                {
                    Name = "Tint",
                    Color = Colors.White
                },
                Foreground = new GaussianBlurEffect
                {
                    Name = "Blur",
                    Source = new CompositionEffectSourceParameter("Backdrop"),
                    BlurAmount = 35.0f,
                    BorderMode = EffectBorderMode.Hard
                }
            };

            var blurEffectFactory = _compositor.CreateEffectFactory(graphicsEffect,
                new[] {"Blur.BlurAmount", "Tint.Color"});

            // Create EffectBrush, BackdropBrush and SpriteVisual
            _brush = blurEffectFactory.CreateBrush();

            var destinationBrush = _compositor.CreateBackdropBrush();
            _brush.SetSourceParameter("Backdrop", destinationBrush);

            var blurSprite = _compositor.CreateSpriteVisual();
            blurSprite.Size = new Vector2(
                (float) BackgroundImage.ActualWidth,
                (float) BackgroundImage.ActualHeight);
            blurSprite.Brush = _brush;

            ElementCompositionPreview.SetElementChildVisual(BackgroundImage, blurSprite);
        }

        private void BackgroundImage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var blurVisual = (SpriteVisual) ElementCompositionPreview.GetElementChildVisual(BackgroundImage);

            if (blurVisual != null)
            {
                blurVisual.Size = e.NewSize.ToVector2();
            }
        }

        #endregion

    }
}