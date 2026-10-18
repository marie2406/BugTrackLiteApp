using BugTrackLite.Views;

namespace BugTrackLite;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Pages ouvertes "par-dessus" les onglets (navigation avec bouton retour).
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("add", typeof(AddTicketPage));
        Routing.RegisterRoute("detail", typeof(TicketDetailPage));
    }
}
