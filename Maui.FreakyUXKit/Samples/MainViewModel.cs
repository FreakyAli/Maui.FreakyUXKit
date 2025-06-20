using CommunityToolkit.Mvvm.Input;
using Maui.FreakyUXKit;
using Samples.Focus;

namespace Samples;

public partial class MainViewModel : FreakyBaseViewModel
{
    public List<CoachmarkAnimationStyle> AnimationTypes { get; set; }

    public MainViewModel()
    {
        AnimationTypes = Enum.GetValues(typeof(CoachmarkAnimationStyle)).Cast<CoachmarkAnimationStyle>().ToList();
    }
    
    [RelayCommand]
    private void ShowNextPage(CoachmarkAnimationStyle selection)
    {
        switch (selection)
        {
            case CoachmarkAnimationStyle.Focus:
               App.CurrentPage.Navigation.PushAsync(new FocusCoachmarkPage()); 
                break;
            default:
                break;
        }
    }
}
