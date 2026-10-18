using BugTrackLite.Models;
using BugTrackLite.ViewModels;

namespace BugTrackLite.Views;

public partial class DonePage : ContentPage
{
    private readonly TicketListViewModel _vm;

    public DonePage(TicketListViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        _vm.Status = TicketStatus.Done;   // colonne "Terminé"
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
