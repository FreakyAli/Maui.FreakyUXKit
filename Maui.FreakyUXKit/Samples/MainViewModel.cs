using CommunityToolkit.Mvvm.Input;

namespace Samples;

public partial class MainViewModel : FreakyBaseViewModel
{
    public List<string> AnimationTypes { get; set; }

    public MainViewModel()
    {
        AnimationTypes =
        [
            AppShell.focus,
            AppShell.arrow,
            AppShell.spotlight,
            AppShell.intro
        ];
    }

    [RelayCommand]
    private async Task ShowNextPage(string selection)
    {
        await Shell.Current.GoToAsync(selection);
    }
}