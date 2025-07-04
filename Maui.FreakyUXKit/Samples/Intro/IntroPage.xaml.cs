namespace Samples.Intro;

public partial class IntroPage : ContentPage
{
	public IntroPage()
	{
		InitializeComponent();
		BindingContext = new IntroViewModel();
	}
}