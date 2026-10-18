using BugTrackLite.Helpers;
using BugTrackLite.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BugTrackLite.ViewModels;

// Écran "Connexion".
public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _auth;
    public LoginViewModel(AuthService auth) => _auth = auth;

    // [ObservableProperty] génère automatiquement la propriété publique (ex. email -> Email).
    [ObservableProperty] private string email = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string emailError = "";
    [ObservableProperty] private string generalError = "";
    [ObservableProperty] private bool isBusy;

    [RelayCommand]
    private async Task LoginAsync()
    {
        EmailError = ""; GeneralError = "";

        if (!AuthService.IsValidEmail(Email))
        {
            EmailError = "Suivez le format donné plus haut pour votre e-mail !";
            return;
        }
        if (string.IsNullOrEmpty(Password))
        {
            GeneralError = "Saisissez votre mot de passe.";
            return;
        }

        IsBusy = true;
        var (ok, error) = await _auth.LoginAsync(Email, Password);
        IsBusy = false;

        if (!ok) { GeneralError = error ?? "Connexion impossible."; return; }
        Password = "";
        await Shell.Current.GoToAsync("//main");   // direction l'accueil
    }

    [RelayCommand]
    private async Task StudentLoginAsync()
    {
        IsBusy = true;
        var (ok, error) = await _auth.LoginStudentAsync();
        IsBusy = false;
        if (!ok) { GeneralError = error ?? "Connexion impossible."; return; }
        await Shell.Current.GoToAsync("//main");
    }

    [RelayCommand]
    private Task ForgotPasswordAsync()
        => Dialogs.AlertAsync("Mot de passe oublié",
            "L'application fonctionne hors-ligne : la réinitialisation par e-mail n'est pas disponible. Créez un nouveau compte ou utilisez le compte étudiant.");

    [RelayCommand]
    private Task GoToRegisterAsync() => Shell.Current.GoToAsync("register");
}
