using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Samples;

public class FreakyBaseViewModel : ObservableObject
{
    public ICommand BackButtonCommand { get; }

    public FreakyBaseViewModel()
    {
        BackButtonCommand = new Command(ExecuteOnBackButtonClicked);
    }

    private async void ExecuteOnBackButtonClicked()
    {
        await App.CurrentNavigation.PopAsync();
    }
}
