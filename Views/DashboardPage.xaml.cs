using BugTrackLite.ViewModels;

namespace BugTrackLite.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _vm;

    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    // Recharge les chiffres chaque fois que l'écran redevient visible.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
