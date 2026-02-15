using System.Windows.Input;

namespace Samples.Focus;

public class FocusCoachmarkViewModel : FreakyBaseViewModel
{
    public ICommand CoachmarkCompletedCommand { get; }
    public FocusCoachmarkViewModel()
    {
        CoachmarkCompletedCommand = new Command(() =>
        {
        });
    }
}