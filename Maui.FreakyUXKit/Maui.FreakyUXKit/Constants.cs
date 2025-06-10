using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Maui.FreakyUXKit;

internal static class Constants
{
    internal const string freakyPopup = "freaky-popup";
    internal const string Coachmarks = "Coachmarks";
    internal static Color backgroundColor = new(0, 0, 0, 180);
    internal static SKColor backgroundSKColor = backgroundColor.ToSKColor();
    internal static Color focusAnimationColor = Colors.Red;
    private static DisplayInfo Display => DeviceDisplay.MainDisplayInfo;

#if NET9_0_OR_GREATER
    internal static Page? MainPage => Application.Current.Windows.FirstOrDefault()?.Page;
#else
    internal static Page? MainPage => Application.Current.MainPage;
#endif
    internal static double Width => Display.Width / Display.Density;
    internal static double Height => Display.Height / Display.Density;
    internal static double Dpi => Display.Density;
}