# Maui.FreakyUXKit

Maui.FreakyUXKit is a lightweight and customisable UX toolkit for .NET MAUI 

<div>
   <a href="https://www.nuget.org/packages/FreakyUXKit"><img src="https://img.shields.io/nuget/v/FreakyUXKit?color=blue&logo=nuget"></a>
   <a href="https://www.nuget.org/packages/FreakyUXKit"><img src="https://img.shields.io/nuget/dt/FreakyUXKit.svg"></a>
   <a href="./LICENSE"><img src="https://img.shields.io/github/license/freakyali/maui.freakyuxkit"></a>
   <a href="https://www.codefactor.io/repository/github/freakyali/maui.freakyuxkit"><img src="https://www.codefactor.io/repository/github/freakyali/maui.freakyuxkit/badge"></a>
   <a href="https://app.fossa.com/projects/git%2Bgithub.com%2FFreakyAli%2FMaui.FreakyUXKit?ref=badge_shield" alt="FOSSA Status"><img src="https://app.fossa.com/api/projects/git%2Bgithub.com%2FFreakyAli%2FMaui.FreakyUXKit.svg?type=shield"/></a>
   </div>

## Platforms

| Support |          OS           |
| ------- | :-------------------: |
| iOS     |      iOS 15.0 +       |
| Android | API 26+ (Marshmallow) |

## Documentation

For more details and API documentation, check our [Wiki](https://github.com/FreakyAli/Maui.FreakyUXKit/wiki)

### Like what you saw? Want to keep this repo alive?

[![](https://miro.medium.com/max/600/0*wrBJU05A3BULKcWA.gif)](https://www.buymeacoffee.com/FreakyAli)

## Previews:

## License

The license for this project can be found [here](https://github.com/FreakyAli/Maui.FreakyUXKit/blob/master/LICENSE)

## Installation

Add our [NuGet](https://www.nuget.org/packages/FreakyUXKit) package or

Run the following command to add our Nuget to your .Net MAUI app:

      Install-Package FreakyUXKit -Version xx.xx.xx

Add the following using statement and Initialization in your MauiProgram:

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

## Activity

Fossa:

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FFreakyAli%2FMaui.FreakyUXKit.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FFreakyAli%2FMaui.FreakyUXKit?ref=badge_large)

Sparkline:

[![Sparkline](https://stars.medv.io/FreakyAli/Maui.FreakyUXKit.svg)](https://stars.medv.io/FreakyAli/Maui.FreakyUXKit)

RepoBeats:

![RepoBeats](https://repobeats.axiom.co/api/embed/62b773582fc822efda7de377fb73c527444ff44d.svg "Repobeats analytics image")
