namespace Samples;

public partial class App : Application
{
	#nullable enable
	internal static Page? CurrentPage => Current?.Windows?.FirstOrDefault()?.Page;
	#nullable disable
	internal static NavigationPage CurrentNavigation { get; set; }

	public App()
	{
		InitializeComponent();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
		CurrentNavigation = new NavigationPage(new MainPage());
		return new Window(CurrentNavigation);
    }
}