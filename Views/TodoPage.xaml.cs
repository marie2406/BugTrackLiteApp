using BugTrackLite.Models;
using BugTrackLite.ViewModels;

namespace BugTrackLite.Views;

public partial class TodoPage : ContentPage
{
    private readonly TicketListViewModel _vm;

    public TodoPage(TicketListViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        _vm.Status = TicketStatus.Todo;   // cette page affiche la colonne "À faire"
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
