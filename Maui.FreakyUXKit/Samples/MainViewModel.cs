using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Samples;

public partial class MainViewModel : FreakyBaseViewModel
{
    [ObservableProperty]
    public ObservableCollection<string> items;
    public MainViewModel()
    {
        Items = new ObservableCollection<string>
            {
                AppShell.FocusView,
            };
    }
}
