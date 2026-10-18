using BugTrackLite.ViewModels;

namespace BugTrackLite.Views;

public partial class TicketDetailPage : ContentPage
{
    public TicketDetailPage(TicketDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
