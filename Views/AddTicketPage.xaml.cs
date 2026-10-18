using BugTrackLite.ViewModels;

namespace BugTrackLite.Views;

public partial class AddTicketPage : ContentPage
{
    public AddTicketPage(AddTicketViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
