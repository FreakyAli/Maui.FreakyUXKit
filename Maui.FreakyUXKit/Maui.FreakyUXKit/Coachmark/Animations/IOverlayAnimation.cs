namespace Maui.FreakyUXKit.Coachmark.Animations;

public interface IOverlayAnimation : ICoachmarkAnimation
{
    /// <summary>
    /// Optional: Apply view-based animation or visual effects to the overlay View itself.
    /// Called once after overlay view is set or changed.
    /// </summary>
    void ApplyToView(View? overlayView);
}