using BugTrackLite.Models;
using BugTrackLite.ViewModels;

namespace BugTrackLite.Views;

public partial class InProgressPage : ContentPage
{
    private readonly TicketListViewModel _vm;

    public InProgressPage(TicketListViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        _vm.Status = TicketStatus.InProgress;   // colonne "En cours"
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
