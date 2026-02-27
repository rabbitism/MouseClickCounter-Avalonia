using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MouseClickCounter.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _clickCount;

    [RelayCommand]
    private void IncrementClick()
    {
        ClickCount++;
    }

    [RelayCommand]
    private void ResetClick()
    {
        ClickCount = 0;
    }
}
