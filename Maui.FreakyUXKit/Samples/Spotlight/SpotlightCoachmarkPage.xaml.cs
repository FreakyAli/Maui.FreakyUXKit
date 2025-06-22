namespace Samples.Spotlight;

public partial class SpotlightCoachmarkPage
{
	public SpotlightCoachmarkPage()
	{
		InitializeComponent();
		BindingContext = new SpotlightViewModel();
	}
}