using CommunityToolkit.Mvvm.Input;
using Maui.FreakyUXKit;

namespace Samples;

public partial class MainViewModel : FreakyBaseViewModel
{
    public List<CoachmarkAnimationStyle> AnimationTypes { get; set; }

    public MainViewModel()
    {
        AnimationTypes = Enum.GetValues<CoachmarkAnimationStyle>().Cast<CoachmarkAnimationStyle>().ToList();
        AnimationTypes.Remove(CoachmarkAnimationStyle.None);
    }

    [RelayCommand]
    private void ShowNextPage(CoachmarkAnimationStyle selection)
    {
        switch (selection)
        {
            case CoachmarkAnimationStyle.Focus:
                Shell.Current.GoToAsync(AppShell.focus);
                break;
            case CoachmarkAnimationStyle.Arrow:
                Shell.Current.GoToAsync(AppShell.arrow);
                break;
            case CoachmarkAnimationStyle.Spotlight:
                Shell.Current.GoToAsync(AppShell.spotlight);
                break;
            default:
                break;
        }
    }
}
