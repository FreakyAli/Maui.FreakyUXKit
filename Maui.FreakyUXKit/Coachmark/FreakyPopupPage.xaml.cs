using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Maui.FreakyUXKit;

public partial class FreakyPopupPage : ContentPage
{
	private readonly IEnumerable<View> _views;
	private int _currentIndex;
	private View _currentTargetView => _views?.ElementAt(_currentIndex);
	private HighlightShape _currentShape;
	private SKRect _currentBounds;
	private readonly List<View> _overlays = new();
	private System.Timers.Timer _animationTimer;
	private float _pulseScale = 1f;
	private bool _pulseGrowing = true;

	public FreakyPopupPage(IEnumerable<View> coachMarkViews)
	{
        _views = coachMarkViews;
		_currentIndex = 0;
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
        
        _currentShape = FreakyCoachmark.GetHighlightShape(_currentTargetView);
        var overlayView = (View)FreakyCoachmark.GetOverlayView(_currentTargetView);
        var highlightAnimation = FreakyCoachmark.GetHighlightAnimation(_currentTargetView);
        var overlayAnimation = FreakyCoachmark.GetOverlayAnimation(_currentTargetView);

        if (_currentTargetView.Handler == null || overlayView == null)
            return;

        var bounds = _currentTargetView.GetRelativeBoundsTo(container);
        _currentBounds = new SKRect(
            (float)bounds.X,
            (float)bounds.Y,
            (float)(bounds.X + bounds.Width),
            (float)(bounds.Y + bounds.Height)
        );
        overlayView.Margin = new Thickness(bounds.X, bounds.Y + bounds.Height + 10, 0, 0);
        overlayView.MaximumWidthRequest = bounds.Width;
        container.Children.Add(overlayView);
        _overlays.Add(overlayView);

        StartHighlightAnimation(highlightAnimation);
        StartOverlayAnimation(overlayAnimation);
        canvasView.InvalidateSurface();
    }

    private void StartOverlayAnimation(OverlayAnimationStyle animationStyle)
    {
        switch (animationStyle)
        {
            case OverlayAnimationStyle.Pulse:
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
        var cornerRadius = FreakyCoachmark.GetHighlightShapeCornerRadius(_currentTargetView);
        canvas.Clear(SKColors.Transparent);
        var info = e.Info;
        var rect = info.Rect;
        var highlightRect = _currentBounds;
        float highX = highlightRect.Left + highlightRect.Width / 2;
        float highY = highlightRect.Top + highlightRect.Height / 2;
        float width = highlightRect.Width * _pulseScale;
        float height = highlightRect.Height * _pulseScale;

        if (_currentTargetView == null)
            return;

        // Step 1: Draw translucent overlay (e.g. 70% opacity black)
        canvas.DrawBasicBackgroundOverlay(rect);

        // Step 2: "Cut out" the highlight area using Clear blend mode
        canvas.DrawHighlightCutOut(_currentShape, highX, highY, width, height, cornerRadius);

        // Step 3: Draw white stroke around the transparent hole
        canvas.DrawHighlightStroke(_currentShape, highX, highY, width, height);
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