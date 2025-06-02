## Installation

Add our [NuGet](https://www.nuget.org/packages/FreakyUXKit) package or

Run the following command to add our NuGet to your .NET MAUI app:

      Install-Package FreakyUXKit -Version xx.xx.xx

Add the following using statement and usage in your MauiProgram:

```c#
using MAUI.FreakyUXKit;
namespace Samples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
                        {
                            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                        });
        builder.UseFreakyUXKit();
        return builder.Build();
     }
 }

```
