using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Maui.FreakyUXKit;

public partial class FreakyPopupPage : ContentPage
{
    private readonly IEnumerable<View> _views;
    private int _currentIndex;
    private SKRect _currentBounds;
    private readonly List<View> _overlays = new();
    
    // Combined animation properties
    private System.Timers.Timer _convergencePulseTimer;
    private float _animationProgress = 0f;
    private float _pulseScale = 1f;
    private bool _pulseGrowing = true;
    private bool _hasRippleCompleted = false;

    // Animation phases
    private const float RipplePhaseEnd = 0.5f;        // 0.0 to 0.5 - Ripple convergence
    private const float TransitionPhaseEnd = 0.6f;    // 0.5 to 0.6 - Brief transition
    private const float PulsePhaseStart = 0.6f;       // 0.6 to ∞ - Continuous pulsing

    #region Properties
    private View CurrentTargetView => _views?.ElementAtOrDefault(_currentIndex) ?? new ContentView();
    private View OverlayView => (View)FreakyCoachmark.GetOverlayView(CurrentTargetView);
    private CoachmarkAnimationStyle CoachmarkAnimation => FreakyCoachmark.GetCoachmarkAnimation(CurrentTargetView);
    private HighlightShape CurrentShape => FreakyCoachmark.GetHighlightShape(CurrentTargetView);
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

        // Reset animation state for each new coach mark
        _hasRippleCompleted = false;
        _animationProgress = 0f;
        _pulseScale = 1f;
        _pulseGrowing = true;

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

        StartAnimating();
        canvasView.InvalidateSurface();
    }

    private void StartAnimating()
    {
        switch (CoachmarkAnimation)
        {
            case CoachmarkAnimationStyle.Wave:
                StartConvergencePulseAnimation();
                break;
            case CoachmarkAnimationStyle.None:
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
        StopAnimating();
    }

    private void StopAnimating()
    {
        switch (CoachmarkAnimation)
        {
            case CoachmarkAnimationStyle.Wave:
                StopConvergencePulseAnimation();
                break;
            case CoachmarkAnimationStyle.None:
            default:
                // No animation
                break;
        }
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

        if (CurrentTargetView == null)
            return;

        // Handle different animation phases
        if (CoachmarkAnimation == CoachmarkAnimationStyle.Wave)
        {
            float width = highlightRect.Width * _pulseScale;
            float height = highlightRect.Height * _pulseScale;
            DrawConvergencePulseAnimation(canvas, rect, highX, highY, width, height);
        }
        else
        {
            // Draw basic translucent overlay for non-animated states
            float width = highlightRect.Width;
            float height = highlightRect.Height;
            canvas.DrawBasicBackgroundOverlay(rect);
            canvas.DrawHighlightCutOut(CurrentShape, highX, highY, width, height, CornerRadius);
            canvas.DrawHighlightStroke(CurrentShape, highX, highY, width, height);
        }
    }

    private void DrawConvergencePulseAnimation(SKCanvas canvas, SKRect rect, float highX, float highY, float width, float height)
    {
        if (_animationProgress <= RipplePhaseEnd)
        {
            // Phase 1: Ripple convergence (use existing ripple logic)
            canvas.DrawReverseRippleToHighlight(rect, CurrentShape, highX, highY, width, height, CornerRadius, _animationProgress);
        }
        else if (_animationProgress <= TransitionPhaseEnd)
        {
            // Phase 2: Brief transition - solid red background
            using var backgroundPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(255, 0, 0), // Red background from ripple
                IsAntialias = true
            };
            canvas.DrawRect(rect, backgroundPaint);
            
            // Cut out highlight area
            canvas.DrawHighlightCutOut(CurrentShape, highX, highY, width, height, CornerRadius);
            
            // Draw stroke
            canvas.DrawHighlightStroke(CurrentShape, highX, highY, width, height);
        }
        else
        {
            // Phase 3: Pulsing highlight on red background
            DrawPulsingHighlight(canvas, rect, highX, highY, width, height);
        }
    }

    private void DrawPulsingHighlight(SKCanvas canvas, SKRect rect, float highX, float highY, float width, float height)
    {
        // Draw red background
        using var backgroundPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 0, 0),
            IsAntialias = true
        };
        canvas.DrawRect(rect, backgroundPaint);

        // Cut out the pulsing highlight area
        canvas.DrawHighlightCutOut(CurrentShape, highX, highY, width, height, CornerRadius);

        // Draw enhanced pulsing stroke with opacity based on pulse scale
        float pulseIntensity = (_pulseScale - 1f) * 10f; // Convert 1.0-1.1 to 0-1 range
        byte strokeAlpha = (byte)(255 - (pulseIntensity * 100)); // Fade stroke as it pulses out
        byte glowAlpha = (byte)(pulseIntensity * 150); // Increase glow as it pulses out

        // Draw outer glow effect
        if (glowAlpha > 0)
        {
            using var glowPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = new SKColor(255, 255, 255, glowAlpha),
                StrokeWidth = 8f * _pulseScale,
                IsAntialias = true
            };
            DrawShapeStroke(canvas, CurrentShape, highX, highY, width, height, glowPaint);
        }

        // Draw main stroke
        using var strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(255, 255, 255, strokeAlpha),
            StrokeWidth = 4f,
            IsAntialias = true
        };
        DrawShapeStroke(canvas, CurrentShape, highX, highY, width, height, strokePaint);
    }

    private void DrawShapeStroke(SKCanvas canvas, HighlightShape shape, float centerX, float centerY, 
        float width, float height, SKPaint paint)
    {
        switch (shape)
        {
            case HighlightShape.Circle:
                float radius = Math.Max(width, height) / 2;
                canvas.DrawCircle(centerX, centerY, radius, paint);
                break;
            case HighlightShape.Ellipse:
                canvas.DrawOval(new SKRect(centerX - width / 2, centerY - height / 2, 
                    centerX + width / 2, centerY + height / 2), paint);
                break;
            case HighlightShape.Rectangle:
                canvas.DrawRect(new SKRect(centerX - width / 2, centerY - height / 2, 
                    centerX + width / 2, centerY + height / 2), paint);
                break;
            default: // RoundRectangle
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(centerX - width / 2, centerY - height / 2, 
                    centerX + width / 2, centerY + height / 2), CornerRadius), paint);
                break;
        }
    }

    private void StartConvergencePulseAnimation()
    {
        _animationProgress = 0f;
        _pulseScale = 1f;
        _pulseGrowing = true;
        _hasRippleCompleted = false;

        _convergencePulseTimer = new System.Timers.Timer(16); // ~60fps
        _convergencePulseTimer.Elapsed += (s, e) =>
        {
            // Update animation progress
            if (_animationProgress <= RipplePhaseEnd)
            {
                // Ripple phase - slower progression
                _animationProgress += 0.008f;
            }
            else if (_animationProgress <= TransitionPhaseEnd)
            {
                // Quick transition phase
                _animationProgress += 0.02f;
            }
            else
            {
                // Pulse phase - update pulse scale
                if (_pulseGrowing)
                {
                    _pulseScale += 0.003f;
                    if (_pulseScale >= 1.1f)
                    {
                        _pulseScale = 1.1f;
                        _pulseGrowing = false;
                    }
                }
                else
                {
                    _pulseScale -= 0.003f;
                    if (_pulseScale <= 1f)
                    {
                        _pulseScale = 1f;
                        _pulseGrowing = true;
                    }
                }
                
                // Keep animation progress above pulse threshold
                if (_animationProgress < 1f)
                    _animationProgress += 0.001f;
            }

            // Mark ripple as completed when we enter pulse phase
            if (_animationProgress > PulsePhaseStart && !_hasRippleCompleted)
            {
                _hasRippleCompleted = true;
            }

            MainThread.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
        };
        _convergencePulseTimer.Start();
    }

    private void StopConvergencePulseAnimation()
    {
        _convergencePulseTimer?.Stop();
        _convergencePulseTimer?.Dispose();
        _convergencePulseTimer = null;
        
        // Reset animation state
        _animationProgress = 0f;
        _pulseScale = 1f;
        _hasRippleCompleted = false;
    }
}