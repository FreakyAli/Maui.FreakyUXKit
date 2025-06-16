using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Layouts;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Maui.FreakyUXKit;

public partial class FreakyPopupPage : Popup
{
    private readonly IEnumerable<View> _views;
    private int _currentIndex;
    private SKRect _currentBounds;
    private readonly List<View> _overlays = new();
    
    // Animation system properties
    private System.Timers.Timer _animationTimer;
    private float _animationProgress = 0f;
    private float _pulseScale = 1f;
    private bool _pulseGrowing = true;
    private bool _hasAnimationPhaseCompleted = false;

    // Animation phases for Focus animation
    private const float FocusRipplePhaseEnd = 0.5f;        // 0.0 to 0.5 - Ripple convergence
    private const float FocusTransitionPhaseEnd = 0.6f;    // 0.5 to 0.6 - Brief transition
    private const float FocusPulsePhaseStart = 0.6f;       // 0.6 to ∞ - Continuous pulsing

    #region Properties
    private CoachmarkPosition PreferredPosition => FreakyCoachmark.GetPreferredPosition(CurrentTargetView);
    private float HighlightPadding => FreakyCoachmark.GetHighlightPadding(CurrentTargetView);
    private float OverlayMargin => FreakyCoachmark.GetOverlayMargin(CurrentTargetView);
    private View CurrentTargetView => _views?.ElementAtOrDefault(_currentIndex) ?? new ContentView();
    private View OverlayView => (View)FreakyCoachmark.GetOverlayView(CurrentTargetView);
    private CoachmarkAnimationStyle CoachmarkAnimation => FreakyCoachmark.GetCoachmarkAnimation(CurrentTargetView);
    private HighlightShape CurrentShape => FreakyCoachmark.GetHighlightShape(CurrentTargetView);
    private float CornerRadius => FreakyCoachmark.GetHighlightShapeCornerRadius(CurrentTargetView);
    private Color FocusAnimationColor => FreakyCoachmark.GetFocusAnimationColor(CurrentTargetView);
    private Color ArrowColor => FreakyCoachmark.GetArrowColor(CurrentTargetView);
    private ArrowStyle ArrowStyle => FreakyCoachmark.GetArrowStyle(CurrentTargetView);
    private float ArrowStrokeWidth => FreakyCoachmark.GetArrowStrokeWidth(CurrentTargetView);
    #endregion

    public FreakyPopupPage(IEnumerable<View> coachMarkViews)
    {
        InitializeComponent();
        _currentIndex = 0;
        _views = coachMarkViews;
        this.Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        await Task.Delay(100);
        ShowCurrentCoachMark();
    }

    private async void OnBackgroundTapped(object sender, EventArgs e)
    {
        await NextCoachMark().ConfigureAwait(false);
    }

    public override Task CloseAsync(CancellationToken token = default)
    {
        this.Loaded -= OnLoaded;
        ClearOverlayViews();
        return base.CloseAsync(token);
    }

    private async Task NextCoachMark()
    {
        _currentIndex++;
        if (_currentIndex >= _views.Count())
        {
            await Constants.MainPage?.ClosePopupAsync();
        }
        else
        {
            ShowCurrentCoachMark();
        }
    }

    private void ShowCurrentCoachMark()
    {
        ClearOverlayViews();
        ResetAnimationState();

        if (CurrentTargetView.Handler == null || OverlayView == null)
            return;

        var bounds = CurrentTargetView.GetRelativeBoundsTo(container);
        
        // Apply highlight padding to expand the highlight area
        var paddedBounds = new Rect(
            bounds.X - HighlightPadding,
            bounds.Y - HighlightPadding,
            bounds.Width + (HighlightPadding * 2),
            bounds.Height + (HighlightPadding * 2)
        );
        
        _currentBounds = new SKRect(
            (float)paddedBounds.X,
            (float)paddedBounds.Y,
            (float)(paddedBounds.X + paddedBounds.Width),
            (float)(paddedBounds.Y + paddedBounds.Height)
        );

        // Calculate overlay size (use original bounds for positioning calculations)
        var originalBounds = CurrentTargetView.GetRelativeBoundsTo(container);
        var overlaySize = MeasureOverlayView();
        var containerBounds = new SKRect(0, 0, (float)container.Width, (float)container.Height);
        
        // Determine final position using original target bounds (not padded)
        var originalSkRect = new SKRect(
            (float)originalBounds.X,
            (float)originalBounds.Y,
            (float)(originalBounds.X + originalBounds.Width),
            (float)(originalBounds.Y + originalBounds.Height)
        );
        var finalPosition = DetermineOverlayPosition(originalSkRect, containerBounds, overlaySize);
        
        // Calculate and apply margin (use original bounds for overlay positioning)
        var margin = finalPosition.CalculateOverlayMargin(originalSkRect, overlaySize, containerBounds, OverlayMargin);
        OverlayView.Margin = margin;
        
        // Set width constraints based on position (use original bounds)
        SetOverlayConstraints(finalPosition, overlaySize, originalSkRect);
        
        container.Children.Add(OverlayView);
        _overlays.Add(OverlayView);

        StartAnimation();
        canvasView.InvalidateSurface();
    }

    private CoachmarkPosition DetermineOverlayPosition(SKRect targetBounds, SKRect containerBounds, Size overlaySize)
    {
        if (PreferredPosition != CoachmarkPosition.Auto)
        {
            // Check if preferred position has enough space
            if (HasEnoughSpace(PreferredPosition, targetBounds, containerBounds, overlaySize))
                return PreferredPosition;
        }
        
        // Fall back to automatic calculation
        return targetBounds.CalculateOptimalPosition(containerBounds, overlaySize, OverlayMargin);
    }

    private bool HasEnoughSpace(CoachmarkPosition position, SKRect targetBounds, SKRect containerBounds, Size overlaySize)
    {
        var margin = OverlayMargin;
        
        return position switch
        {
            CoachmarkPosition.Top => targetBounds.Top >= overlaySize.Height + margin,
            CoachmarkPosition.Bottom => containerBounds.Height - targetBounds.Bottom >= overlaySize.Height + margin,
            CoachmarkPosition.Left => targetBounds.Left >= overlaySize.Width + margin,
            CoachmarkPosition.Right => containerBounds.Width - targetBounds.Right >= overlaySize.Width + margin,
            CoachmarkPosition.TopLeft => targetBounds.Top >= overlaySize.Height + margin && 
                                       targetBounds.Left >= overlaySize.Width + margin,
            CoachmarkPosition.TopRight => targetBounds.Top >= overlaySize.Height + margin && 
                                        containerBounds.Width - targetBounds.Right >= overlaySize.Width + margin,
            CoachmarkPosition.BottomLeft => containerBounds.Height - targetBounds.Bottom >= overlaySize.Height + margin && 
                                          targetBounds.Left >= overlaySize.Width + margin,
            CoachmarkPosition.BottomRight => containerBounds.Height - targetBounds.Bottom >= overlaySize.Height + margin && 
                                           containerBounds.Width - targetBounds.Right >= overlaySize.Width + margin,
            _ => true
        };
    }

    private Size MeasureOverlayView()
    {
        if (OverlayView == null) return new Size(200, 100);
        
        // Measure the overlay view to get its desired size
        var widthConstraint = container.Width * 0.8; // Max 80% of container width
        var heightConstraint = container.Height * 0.6; // Max 60% of container height
        
        var size = OverlayView.Measure(widthConstraint, heightConstraint);
        return new Size(
            Math.Min(size.Width, widthConstraint),
            Math.Min(size.Height, heightConstraint)
        );
    }

    private void SetOverlayConstraints(CoachmarkPosition position, Size overlaySize, SKRect originalBounds)
    {
        switch (position)
        {
            case CoachmarkPosition.Left:
            case CoachmarkPosition.Right:
            case CoachmarkPosition.TopLeft:
            case CoachmarkPosition.TopRight:
            case CoachmarkPosition.BottomLeft:
            case CoachmarkPosition.BottomRight:
                OverlayView.MaximumWidthRequest = overlaySize.Width;
                break;
            default:
                OverlayView.MaximumWidthRequest = originalBounds.Width;
                break;
        }
    }

    private void ResetAnimationState()
    {
        _hasAnimationPhaseCompleted = false;
        _animationProgress = 0f;
        _pulseScale = 1f;
        _pulseGrowing = true;
    }

    private void StartAnimation()
    {
        switch (CoachmarkAnimation)
        {
            case CoachmarkAnimationStyle.Focus:
            case CoachmarkAnimationStyle.Ripple:
            case CoachmarkAnimationStyle.Pulse:
            case CoachmarkAnimationStyle.Spotlight:
            case CoachmarkAnimationStyle.Morph:
                StartFocusAnimation(); // reuse timer system
                break;
            case CoachmarkAnimationStyle.Arrow:
                // Arrow is static, no animation timer needed
                canvasView.InvalidateSurface();
                break;
            case CoachmarkAnimationStyle.None:
            default:
                break;
        }
    }

    private void StopAnimation()
    {
        StopFocusAnimation();
    }

    private void ClearOverlayViews()
    {
        foreach (var view in _overlays)
        {
            container.Children.Remove(view);
        }
        _overlays.Clear();
        StopAnimation();
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

        if (CurrentTargetView == null || OverlayView == null)
            return;

        // Route to appropriate animation renderer
        switch (CoachmarkAnimation)
        {
            case CoachmarkAnimationStyle.Focus:
                RenderFocusAnimation(canvas, rect, highX, highY, highlightRect);
                break;
            case CoachmarkAnimationStyle.Ripple:
                RenderRippleAnimation(canvas, rect, highX, highY, highlightRect);
                break;
            case CoachmarkAnimationStyle.Pulse:
                RenderPulseAnimation(canvas, rect, highX, highY, highlightRect);
                break;
            case CoachmarkAnimationStyle.Spotlight:
                RenderSpotlightAnimation(canvas, rect, highX, highY, highlightRect);
                break;
            case CoachmarkAnimationStyle.Arrow:
                var overlayRect = OverlayView.GetRelativeBoundsTo(container);
                var skOverlayRect = overlayRect.ToSKRect();
                RenderStaticHighlight(canvas, rect, highX, highY, highlightRect);
                RenderArrowPointer(canvas, highlightRect, skOverlayRect);
                break;
            case CoachmarkAnimationStyle.Morph:
                RenderMorphingHighlight(canvas, rect, highX, highY, highlightRect);
                break;
            case CoachmarkAnimationStyle.None:
            default:
                RenderStaticHighlight(canvas, rect, highX, highY, highlightRect);
                break;
        }

    }

    private void RenderStaticHighlight(SKCanvas canvas, SKRect rect, float highX, float highY, SKRect highlightRect)
    {
        float width = highlightRect.Width;
        float height = highlightRect.Height;
        canvas.DrawBasicBackgroundOverlay(rect);
        canvas.DrawHighlightCutOut(CurrentShape, highX, highY, width, height, CornerRadius);
        canvas.DrawHighlightStroke(CurrentShape, highX, highY, width, height);
    }

    private void RenderFocusAnimation(SKCanvas canvas, SKRect rect, float highX, float highY, SKRect highlightRect)
    {
        float width = highlightRect.Width * _pulseScale;
        float height = highlightRect.Height * _pulseScale;

        if (_animationProgress <= FocusRipplePhaseEnd)
        {
            // Phase 1: Focus ripple convergence
            canvas.DrawFocusRippleEffect(rect, CurrentShape, highX, highY, width, height, CornerRadius, _animationProgress, FocusAnimationColor);
        }
        else if (_animationProgress <= FocusTransitionPhaseEnd)
        {
            // Phase 2: Brief transition - solid focus background
            DrawFocusTransition(canvas, rect, highX, highY, width, height);
        }
        else
        {
            // Phase 3: Pulsing highlight on focus background
            DrawFocusPulsingHighlight(canvas, rect, highX, highY, width, height);
        }
    }
    
    private void RenderRippleAnimation(SKCanvas canvas, SKRect rect, float x, float y, SKRect bounds)
    {
        float rippleRadius = bounds.Width * (_animationProgress * 1.5f);
        canvas.DrawBasicBackgroundOverlay(rect);
        canvas.DrawRipple(x, y, rippleRadius, FocusAnimationColor.ToSKColor());
    }

    private void RenderPulseAnimation(SKCanvas canvas, SKRect rect, float x, float y, SKRect bounds)
    {
        float width = bounds.Width * _pulseScale;
        float height = bounds.Height * _pulseScale;
        canvas.DrawBasicBackgroundOverlay(rect);
        canvas.DrawHighlightCutOut(CurrentShape, x, y, width, height, CornerRadius);
        canvas.DrawHighlightStroke(CurrentShape, x, y, width, height);
    }

    private void RenderSpotlightAnimation(SKCanvas canvas, SKRect rect, float x, float y, SKRect bounds)
    {
        canvas.DrawDarkOverlayWithSpotlight(x, y, bounds.Width, bounds.Height);
    }
    
    private SKPoint GetClosestPointOnRect(SKRect rect, SKPoint point)
    {
        // Clamp the point coordinates inside the rect
        float x = MathF.Max(rect.Left, MathF.Min(point.X, rect.Right));
        float y = MathF.Max(rect.Top, MathF.Min(point.Y, rect.Bottom));

        // If point is inside rect, find closest edge instead
        if (rect.Contains(point))
        {
            // Distances to each edge
            float distLeft = MathF.Abs(point.X - rect.Left);
            float distRight = MathF.Abs(rect.Right - point.X);
            float distTop = MathF.Abs(point.Y - rect.Top);
            float distBottom = MathF.Abs(rect.Bottom - point.Y);

            float minDist = MathF.Min(MathF.Min(distLeft, distRight), MathF.Min(distTop, distBottom));

            if (minDist == distLeft) return new SKPoint(rect.Left, point.Y);
            if (minDist == distRight) return new SKPoint(rect.Right, point.Y);
            if (minDist == distTop) return new SKPoint(point.X, rect.Top);
            return new SKPoint(point.X, rect.Bottom);
        }

        // If outside rect, clamping suffices (point on edge or corner)
        return new SKPoint(x, y);
    }


    private void RenderArrowPointer(SKCanvas canvas, SKRect highlightRect, SKRect overlayRect, float paddingPercent = 0.2f)
    {
        // Centers
        var overlayCenter = new SKPoint(overlayRect.MidX, overlayRect.MidY);
        var highlightCenter = new SKPoint(highlightRect.MidX, highlightRect.MidY);

        // Direction vector and distance from overlay center to highlight center
        var dx = highlightCenter.X - overlayCenter.X;
        var dy = highlightCenter.Y - overlayCenter.Y;
        var totalDistance = MathF.Sqrt(dx * dx + dy * dy);
        if (totalDistance == 0)
            return;

        var direction = new SKPoint(dx / totalDistance, dy / totalDistance);

        // Find intersection point of the line from overlayCenter toward highlightCenter with highlightRect edge
        var edgePoint = GetClosestPointOnRect(highlightRect, overlayCenter);

        // Distance from overlayCenter to edgePoint
        var distToEdge = MathF.Sqrt((edgePoint.X - overlayCenter.X) * (edgePoint.X - overlayCenter.X) +
                                (edgePoint.Y - overlayCenter.Y) * (edgePoint.Y - overlayCenter.Y));

        // Calculate start point - padded 20% away from overlayCenter along direction
        var start = new SKPoint(
            overlayCenter.X + direction.X * (totalDistance * paddingPercent),
            overlayCenter.Y + direction.Y * (totalDistance * paddingPercent));

        // Calculate end point - 20% *before* edgePoint along the direction vector
        var end = new SKPoint(
            overlayCenter.X + direction.X * (distToEdge * (1 - paddingPercent)),
            overlayCenter.Y + direction.Y * (distToEdge * (1 - paddingPercent)));

        canvas.DrawArrow(start, end, ArrowColor.ToSKColor(), ArrowStyle, ArrowStrokeWidth);
    }

    private void RenderMorphingHighlight(SKCanvas canvas, SKRect rect, float x, float y, SKRect bounds)
    {
        float morphProgress = (float)Math.Sin(_animationProgress * Math.PI * 2) * 0.1f;
        float width = bounds.Width * (1 + morphProgress);
        float height = bounds.Height * (1 - morphProgress);
        canvas.DrawBasicBackgroundOverlay(rect);
        canvas.DrawHighlightCutOut(CurrentShape, x, y, width, height, CornerRadius);
        canvas.DrawHighlightStroke(CurrentShape, x, y, width, height);
    }

    private void DrawFocusTransition(SKCanvas canvas, SKRect rect, float highX, float highY, float width, float height)
    {
        using var backgroundPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = FocusAnimationColor.ToSKColor(), // Focus color background
            IsAntialias = true
        };
        canvas.DrawRect(rect, backgroundPaint);

        // Cut out highlight area
        canvas.DrawHighlightCutOut(CurrentShape, highX, highY, width, height, CornerRadius);

        // Draw stroke
        canvas.DrawHighlightStroke(CurrentShape, highX, highY, width, height);
    }

    private void DrawFocusPulsingHighlight(SKCanvas canvas, SKRect rect, float highX, float highY, float width, float height)
    {
        // Draw focus background
        using var backgroundPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = FocusAnimationColor.ToSKColor(),
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

    #region Focus Animation Implementation
    private void StartFocusAnimation()
    {
        _animationProgress = 0f;
        _pulseScale = 1f;
        _pulseGrowing = true;
        _hasAnimationPhaseCompleted = false;

        _animationTimer = new System.Timers.Timer(16); // ~60fps
        _animationTimer.Elapsed += UpdateFocusAnimation;
        _animationTimer.Start();
    }

    private void UpdateFocusAnimation(object sender, System.Timers.ElapsedEventArgs e)
    {
        // Update animation progress
        if (_animationProgress <= FocusRipplePhaseEnd)
        {
            // Ripple phase - slower progression
            _animationProgress += 0.008f;
        }
        else if (_animationProgress <= FocusTransitionPhaseEnd)
        {
            // Quick transition phase
            _animationProgress += 0.02f;
        }
        else
        {
            // Pulse phase - update pulse scale
            UpdateFocusPulseScale();
            
            // Keep animation progress above pulse threshold
            if (_animationProgress < 1f)
                _animationProgress += 0.001f;
        }

        // Mark animation phase as completed when we enter pulse phase
        if (_animationProgress > FocusPulsePhaseStart && !_hasAnimationPhaseCompleted)
        {
            _hasAnimationPhaseCompleted = true;
        }

        MainThread.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
    }

    private void UpdateFocusPulseScale()
    {
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
    }

    private void StopFocusAnimation()
    {
        _animationTimer?.Stop();
        _animationTimer?.Dispose();
        _animationTimer = null;
        
        ResetAnimationState();
    }
    #endregion
    
}