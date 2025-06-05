using SkiaSharp;

namespace Maui.FreakyUXKit.Coachmark.Animations;

public enum CoachmarkAnimationPhase
{
    None,
    // Initial animation (like fade-in)
    Enter,
    // Continuous animation (like pulse)
    Loop,
    // Ending animation (optional)
    Exit
}