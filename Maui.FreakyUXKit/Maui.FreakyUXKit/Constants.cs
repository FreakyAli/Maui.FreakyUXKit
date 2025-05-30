namespace Maui.FreakyUXKit;
internal static class Constants
{
    private static DisplayInfo Display => DeviceDisplay.MainDisplayInfo;
    internal static Page? MainPage => Application.Current.Windows.FirstOrDefault()?.Page;
    internal static double Width => Display.Width/Display.Density;
    internal static double Height => Display.Height/Display.Density;
    internal static double Dpi => Display.Density;
}