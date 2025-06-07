namespace Samples.Focus;

public partial class FocusCoachmarkPage : FreakyBaseContentPage
{
	public FocusCoachmarkPage()
	{
		InitializeComponent();
		BindingContext = new FocusCoachmarkViewModel();
	}
}