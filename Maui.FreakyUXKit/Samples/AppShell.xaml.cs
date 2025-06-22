namespace Samples;
public partial class AppShell : Shell
{
    internal const string arrow = "arrow";
    internal const string focus = "focus";
    internal const string spotlight = "spotlight";

    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(arrow, typeof(Arrow.ArrowCoachmarkPage));
        Routing.RegisterRoute(focus, typeof(Focus.FocusCoachmarkPage));
        Routing.RegisterRoute(spotlight, typeof(Spotlight.SpotlightCoachmarkPage));
    }
}