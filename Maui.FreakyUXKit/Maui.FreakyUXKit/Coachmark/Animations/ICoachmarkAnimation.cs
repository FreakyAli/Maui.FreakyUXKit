using SkiaSharp;

namespace Maui.FreakyUXKit.Coachmark.Animations;

public interface ICoachmarkAnimation : IDisposable
{
    /// <summary>
    /// Event fired when this animation completes a non-looping phase.
    /// </summary>
    event Action? AnimationCompleted;

    /// <summary>
    /// Start animation with specified phase.
    /// </summary>
    void Start(CoachmarkAnimationPhase phase);

    /// <summary>
    /// Stop animation (pause or terminate).
    /// </summary>
    void Stop();

    /// <summary>
    /// Called periodically with delta time (seconds) to update internal state.
    /// </summary>
    void UpdateFrame(float deltaTime);

    /// <summary>
    /// Render animation to the canvas with the given bounds.
    /// </summary>
    void Render(SKCanvas canvas, SKRect bounds);
}