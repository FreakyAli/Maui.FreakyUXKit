using SkiaSharp;

namespace Maui.FreakyUXKit.Coachmark.Animations;

public class CompositeCoachmarkAnimation : IDisposable
{
    private readonly ICoachmarkAnimation? _highlightAnimation;
    private readonly IOverlayAnimation? _overlayAnimation;

    public event Action? AnimationCompleted;

    public CompositeCoachmarkAnimation(
        ICoachmarkAnimation? highlightAnimation,
        IOverlayAnimation? overlayAnimation)
    {
        _highlightAnimation = highlightAnimation;
        _overlayAnimation = overlayAnimation;

        if (_highlightAnimation != null)
            _highlightAnimation.AnimationCompleted += OnChildAnimationCompleted;

        if (_overlayAnimation != null)
            _overlayAnimation.AnimationCompleted += OnChildAnimationCompleted;
    }

    public void Start(CoachmarkAnimationPhase phase)
    {
        _highlightAnimation?.Start(phase);
        _overlayAnimation?.Start(phase);
    }

    public void Stop()
    {
        _highlightAnimation?.Stop();
        _overlayAnimation?.Stop();
    }

    public void UpdateFrame(float deltaTime)
    {
        _highlightAnimation?.UpdateFrame(deltaTime);
        _overlayAnimation?.UpdateFrame(deltaTime);
    }

    public void Render(SKCanvas canvas, SKRect highlightBounds, SKRect overlayBounds)
    {
        _highlightAnimation?.Render(canvas, highlightBounds);
        _overlayAnimation?.Render(canvas, overlayBounds);
    }

    public void ApplyToOverlayView(View? overlayView)
    {
        _overlayAnimation?.ApplyToView(overlayView);
    }

    private void OnChildAnimationCompleted()
    {
        AnimationCompleted?.Invoke();
    }

    #region IDisposable

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CompositeCoachmarkAnimation()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_highlightAnimation != null)
        {
            _highlightAnimation.AnimationCompleted -= OnChildAnimationCompleted;
            _highlightAnimation.Dispose();
        }

        if (_overlayAnimation != null)
        {
            _overlayAnimation.AnimationCompleted -= OnChildAnimationCompleted;
            _overlayAnimation.Dispose();
        }
    }
    #endregion
}
