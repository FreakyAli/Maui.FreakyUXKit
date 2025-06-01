using SkiaSharp;

namespace Maui.FreakyUXKit;
internal static class Constants
{
    internal static Color backgroundColor = new(0, 0, 0, 180);
    internal static SKColor backgroundSKColor = new(0, 0, 0, 180);
    internal static Color focusAnimationColor = Colors.Red;
    private static DisplayInfo Display => DeviceDisplay.MainDisplayInfo;
    internal static Page? MainPage => Application.Current.Windows.FirstOrDefault()?.Page;
    internal static double Width => Display.Width/Display.Density;
    internal static double Height => Display.Height/Display.Density;
    internal static double Dpi => Display.Density;
}