﻿using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MediaSample.Annotations;
using MediaSample.Services;
using Microsoft.Graphics.Canvas.Effects;

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
            _compositor = Window.Current.Compositor;
            BlurBackground();
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

            // Play the forward animation
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("HeroImage");
            if (animation != null)
            {
                HeroImage.Opacity = 0;
                HeroImage.ImageOpened += (sender, args) =>
                {
                    animation.TryStart(HeroImage);
                    HeroImage.Opacity = 1;
                };
            }

            base.OnNavigatedTo(e);
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs backRequestedEventArgs)
        {
            // Setup the back navigation
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackHeroImage", HeroImage);

            // Navigation
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            Frame.GoBack();
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