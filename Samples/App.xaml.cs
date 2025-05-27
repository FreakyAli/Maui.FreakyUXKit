namespace Samples;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}
#nullable enable
	protected override Window CreateWindow(IActivationState? _)
	{
		return new Window(new MainPage());
	}
}