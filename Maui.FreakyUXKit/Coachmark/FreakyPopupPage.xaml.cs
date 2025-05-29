using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Maui.FreakyUXKit;

public partial class FreakyPopupPage : ContentPage
{
    private readonly IEnumerable<View> _views;
    private int _currentIndex;
    private SKRect _currentBounds;
    private readonly List<View> _overlays = new();
    private System.Timers.Timer _animationTimer;
    private float _pulseScale = 1f;
    private bool _pulseGrowing = true;

    #region Properties
    private View CurrentTargetView => _views?.ElementAt(_currentIndex);
    private View OverlayView => (View)FreakyCoachmark.GetOverlayView(CurrentTargetView);
    private HighlightAnimationStyle HighlightAnimation => FreakyCoachmark.GetHighlightAnimation(CurrentTargetView);
    private HighlightShape CurrentShape => FreakyCoachmark.GetHighlightShape(CurrentTargetView);
    private OverlayAnimationStyle OverlayAnimation => FreakyCoachmark.GetOverlayAnimation(CurrentTargetView);
    private float CornerRadius => FreakyCoachmark.GetHighlightShapeCornerRadius(CurrentTargetView);
    #endregion

    public FreakyPopupPage(IEnumerable<View> coachMarkViews)
    {
        _currentIndex = 0;
        _views = coachMarkViews;
        InitializeComponent();
    }

    private async void OnBackgroundTapped(object sender, EventArgs e)
    {
        await NextCoachMark().ConfigureAwait(false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(100);
        ShowCurrentCoachMark();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ClearOverlayViews();
    }

    private async Task NextCoachMark()
    {
        _currentIndex++;
        if (_currentIndex >= _views.Count())
        {
            await Constants.MainPage?.Navigation.PopModalAsync(false);
        }
        else
        {
            ShowCurrentCoachMark();
        }
    }

    private void ShowCurrentCoachMark()
    {
        ClearOverlayViews();

        if (CurrentTargetView.Handler == null || OverlayView == null)
            return;

        var bounds = CurrentTargetView.GetRelativeBoundsTo(container);
        _currentBounds = new SKRect(
            (float)bounds.X,
            (float)bounds.Y,
            (float)(bounds.X + bounds.Width),
            (float)(bounds.Y + bounds.Height)
        );
        OverlayView.Margin = new Thickness(bounds.X, bounds.Y + bounds.Height + 10, 0, 0);
        OverlayView.MaximumWidthRequest = bounds.Width;
        container.Children.Add(OverlayView);
        _overlays.Add(OverlayView);

        StartHighlightAnimation(HighlightAnimation);
        StartOverlayAnimation(OverlayAnimation);
        canvasView.InvalidateSurface();
    }

    private void StartOverlayAnimation(OverlayAnimationStyle animationStyle)
    {
        switch (animationStyle)
        {
            case OverlayAnimationStyle.Pulse:
                break;
            case OverlayAnimationStyle.Ripple:
                break;
            case OverlayAnimationStyle.None:
            default:
                // No animation
                break;
        }
    }

    private void StartHighlightAnimation(HighlightAnimationStyle animationStyle)
    {
        switch (animationStyle)
        {
            case HighlightAnimationStyle.Pulse:
                StartHighlightPulse();
                break;
            case HighlightAnimationStyle.None:
            default:
                // No animation
                break;
        }
    }

    private void ClearOverlayViews()
    {
        foreach (var view in _overlays)
        {
            container.Children.Remove(view);
        }
        _overlays.Clear();
        StopOverlayAnimation();
        StopHighlightAnimaion();
    }

    private void StopHighlightAnimaion()
    {
        StopHighlightPulse();
    }

    private void StopOverlayAnimation()
    {

    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        var info = e.Info;
        var rect = info.Rect;
        var highlightRect = _currentBounds;
        float highX = highlightRect.Left + highlightRect.Width / 2;
        float highY = highlightRect.Top + highlightRect.Height / 2;
        float width = highlightRect.Width * _pulseScale;
        float height = highlightRect.Height * _pulseScale;


        if (CurrentTargetView == null)
            return;

        // Step 1: Draw translucent overlay (e.g. 70% opacity black)
        canvas.DrawBasicBackgroundOverlay(rect);

        // Step 2: "Cut out" the highlight area using Clear blend mode
        canvas.DrawHighlightCutOut(CurrentShape, highX, highY, width, height, CornerRadius);

        // Step 3: Draw white stroke around the transparent hole
        canvas.DrawHighlightStroke(CurrentShape, highX, highY, width, height);
    }

    private void StartHighlightPulse()
    {
        _pulseScale = 1f;
        _pulseGrowing = true;
        _animationTimer = new System.Timers.Timer(16); // ~60fps
        _animationTimer.Elapsed += (s, e) =>
        {
            if (_pulseGrowing)
                _pulseScale += 0.005f;
            else
                _pulseScale -= 0.005f;

            if (_pulseScale > 1.1f)
                _pulseGrowing = false;
            else if (_pulseScale < 1f)
                _pulseGrowing = true;

            MainThread.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
        };
        _animationTimer.Start();
    }

    private void StopHighlightPulse()
    {
        _animationTimer?.Stop();
        _animationTimer?.Dispose();
        _animationTimer = null;
        _pulseScale = 1f;
    }
}