using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Samples;

public class FreakyBaseViewModel : ObservableObject
{
    public ICommand BackButtonCommand { get; }
    public ICommand HireCommand { get; }
    public ICommand FollowCommand { get; }

    public FreakyBaseViewModel()
    {
        BackButtonCommand = new AsyncRelayCommand(ExecuteOnBackButtonClicked);
        HireCommand = new AsyncRelayCommand(ExecuteOnHireButtonClicked);
        FollowCommand = new AsyncRelayCommand(ExecuteOnFollowButtonClicked);
    }

    private async Task ExecuteOnHireButtonClicked()
    {
        var link = "https://www.linkedin.com/in/freakyali/";
        Uri uri = new(link);
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
    }

    private async Task ExecuteOnFollowButtonClicked()
    {
        var link = "https://github.com/FreakyAli";
        Uri uri = new(link);
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
    }

    private async Task ExecuteOnBackButtonClicked()
    {
        await Shell.Current.GoToAsync("..");
    }
}
