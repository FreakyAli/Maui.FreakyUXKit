using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Maui.FreakyUXKit;

public static class Hosting
{
    public static MauiAppBuilder UseFreakyUXKit(this MauiAppBuilder builder)
    {
#pragma warning disable CA1416 // Platform constraints are enforced by MAUI at the project/TFM level
        builder
            .UseMauiCommunityToolkit()
            .UseSkiaSharp();
#pragma warning restore CA1416
        return builder;
    }
}