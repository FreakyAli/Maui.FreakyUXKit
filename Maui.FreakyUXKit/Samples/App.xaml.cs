namespace Samples;

public partial class App : Application
{
	#nullable enable
	internal static Page? CurrentPage => Current?.Windows?.FirstOrDefault()?.Page;
	#nullable disable

	public App()
	{
		InitializeComponent();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
		return new Window(new AppShell());
    }
}