using BugTrackLite.Services;

namespace BugTrackLite;

public partial class App : Application
{
    private readonly AuthService _auth;

    public App(AuthService auth)
    {
        InitializeComponent();
        _auth = auth;
        // Les maquettes sont en thème clair : on force ce thème pour garder des couleurs lisibles.
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = new AppShell();
        // Si l'utilisateur est déjà connecté, on saute l'écran de connexion (Items[1] = barre d'onglets).
        if (_auth.IsLoggedIn)
            shell.CurrentItem = shell.Items[1];
        return new Window(shell);
    }
}
