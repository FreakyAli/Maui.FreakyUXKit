using SkiaSharp;

namespace Maui.FreakyUXKit.Coachmark.Animations;

public class FocusCovergenceOverlayAnimation(SKRect canvasRect, HighlightShape shape, float centerX, float centerY,
    float targetWidth, float targetHeight, float cornerRadius, Color focusColor) : BaseCoachmarkAnimation
{
    private float _elapsed = 0f;
    private readonly HighlightShape _shape = shape;
    private readonly float _centerX = centerX;
    private readonly float _centerY = centerY;
    private readonly float _targetWidth = targetWidth;
    private readonly float _targetHeight = targetHeight;
    private readonly float _cornerRadius = cornerRadius;
    private readonly SKRect _canvasRect = canvasRect;
    private readonly Color _focusColor = focusColor;

    public override void Start(CoachmarkAnimationPhase phase)
    {
        base.Start(phase);
        _elapsed = 0f;
    }

    public override void Render(SKCanvas canvas, SKRect bounds)
    {
        canvas.DrawFocusConvergenceWave(_canvasRect, _shape, _centerX, _centerY,
            _targetWidth, _targetHeight, _cornerRadius, _elapsed, _focusColor);
    }

    public override void UpdateFrame(float deltaTime)
    {
    }
}