using Maui.FreakyUXKit.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Maui.FreakyUXKit;

public partial class FreakyPopupPage : ContentPage
{
	private readonly IEnumerable<View> _views;
	private int _currentIndex;
	private View _currentTargetView;
	private HighlightShape _currentShape;
	private SKRect _currentBounds;
	private readonly List<View> _overlays = new();
	private System.Timers.Timer _animationTimer;
	private float _pulseScale = 1f;
	private bool _pulseGrowing = true;

	public FreakyPopupPage(IEnumerable<View> coachMarkViews)
	{
		InitializeComponent();
		_views = coachMarkViews;
		_currentIndex = 0;
	}

    private async void OnBackgroundTapped(object sender, EventArgs e)
    {
        await NextCoachMark().ConfigureAwait(false);
    }
	
	 protected override async void OnAppearing()
    {
        await Task.Delay(100);
        ShowCurrentCoachMark();
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopPulse();
        ClearOverlayViews();
    }

    private async Task NextCoachMark()
    {
        _currentIndex++;
        if (_currentIndex >= _views.Count())
        {
            await Constants.MainPage?.Navigation.PopModalAsync();
        }
        else
        {
            ShowCurrentCoachMark();
        }
    }

    private void ShowCurrentCoachMark()
    {
        ClearOverlayViews();

        _currentTargetView = _views.ElementAt(_currentIndex);
        _currentShape = FreakyCoachmark.GetHighlightShape(_currentTargetView);
        var overlayView = FreakyCoachmark.GetOverlayView(_currentTargetView) as View;

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

        StartPulse();
        canvas.InvalidateSurface();
    }

    private void ClearOverlayViews()
    {
        foreach (var view in _overlays)
        {
            container.Children.Remove(view);
        }
        _overlays.Clear();
        StopPulse();
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        if (_currentTargetView == null)
            return;

        // Step 1: Draw translucent overlay (e.g. 70% opacity black)
        using (var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(0, 0, 0, 180),  // semi-transparent black
            IsAntialias = true
        })
        {
            canvas.DrawRect(e.Info.Rect, paint);
        }

        // Step 2: "Cut out" the highlight area using Clear blend mode
        using (var clearPaint = new SKPaint
        {
            BlendMode = SKBlendMode.Clear,
            IsAntialias = true
        })
        {
            var highlightRect = _currentBounds;
            float cx = highlightRect.Left + highlightRect.Width / 2;
            float cy = highlightRect.Top + highlightRect.Height / 2;
            float w = highlightRect.Width * _pulseScale;
            float h = highlightRect.Height * _pulseScale;

            switch (_currentShape)
            {
                case HighlightShape.Circle:
                    float radius = Math.Max(w, h) / 2;
                    canvas.DrawCircle(cx, cy, radius, clearPaint);
                    break;
                case HighlightShape.Ellipse:
                    canvas.DrawOval(new SKRect(cx - w / 2, cy - h / 2, cx + w / 2, cy + h / 2), clearPaint);
                    break;
                case HighlightShape.Rectangle:
                    canvas.DrawRect(new SKRect(cx - w / 2, cy - h / 2, cx + w / 2, cy + h / 2), clearPaint);
                    break;
                default:
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(cx - w / 2, cy - h / 2, cx + w / 2, cy + h / 2), 20), clearPaint);
                    break;
            }
        }

        // Step 3: Draw white stroke around the transparent hole
        using (var strokePaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 4,
            Color = SKColors.White
        })
        {
            var highlightRect = _currentBounds;
            float cx = highlightRect.Left + highlightRect.Width / 2;
            float cy = highlightRect.Top + highlightRect.Height / 2;
            float w = highlightRect.Width * _pulseScale;
            float h = highlightRect.Height * _pulseScale;

            switch (_currentShape)
            {
                case HighlightShape.Circle:
                    float radius = Math.Max(w, h) / 2;
                    canvas.DrawCircle(cx, cy, radius, strokePaint);
                    break;
                case HighlightShape.Ellipse:
                    canvas.DrawOval(new SKRect(cx - w / 2, cy - h / 2, cx + w / 2, cy + h / 2), strokePaint);
                    break;
                case HighlightShape.Rectangle:
                    canvas.DrawRect(new SKRect(cx - w / 2, cy - h / 2, cx + w / 2, cy + h / 2), strokePaint);
                    break;
                default:
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(cx - w / 2, cy - h / 2, cx + w / 2, cy + h / 2), 20), strokePaint);
                    break;
            }
        }
    }

    private void StartPulse()
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

            MainThread.BeginInvokeOnMainThread(() => canvas.InvalidateSurface());
        };
        _animationTimer.Start();
    }

    private void StopPulse()
    {
        _animationTimer?.Stop();
        _animationTimer?.Dispose();
        _animationTimer = null;
        _pulseScale = 1f;
    }
}