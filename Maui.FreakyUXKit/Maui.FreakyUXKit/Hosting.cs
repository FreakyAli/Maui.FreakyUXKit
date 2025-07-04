using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Maui.FreakyUXKit;

public static class Hosting
{
    public static MauiAppBuilder UseFreakyUXKit(this MauiAppBuilder builder)
    {
        builder
            .UseSkiaSharp();
        return builder;
    }
}