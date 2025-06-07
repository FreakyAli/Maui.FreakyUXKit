namespace Samples;

public partial class AppShell : Shell
{
    internal const string FocusView = "Focus";

    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(FocusView, typeof(Focus.FocusCoachmarkPage));
    }
}