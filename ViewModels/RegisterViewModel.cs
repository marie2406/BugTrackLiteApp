using BugTrackLite.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BugTrackLite.ViewModels;

// Écran "Inscription".
public partial class RegisterViewModel : ObservableObject
{
    private readonly AuthService _auth;
    public RegisterViewModel(AuthService auth) => _auth = auth;

    [ObservableProperty] private string fullName = "";
    [ObservableProperty] private string projectName = "";
    [ObservableProperty] private string email = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string confirmPassword = "";

    [ObservableProperty] private string nameError = "";
    [ObservableProperty] private string emailError = "";
    [ObservableProperty] private string passwordError = "";
    [ObservableProperty] private string confirmError = "";
    [ObservableProperty] private string generalError = "";
    [ObservableProperty] private bool isBusy;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        NameError = EmailError = PasswordError = ConfirmError = GeneralError = "";
        bool valid = true;

        if (string.IsNullOrWhiteSpace(FullName)) { NameError = "Ne laissez pas ce champ vide !"; valid = false; }
        if (!AuthService.IsValidEmail(Email)) { EmailError = "Suivez le format donné plus haut pour votre e-mail !"; valid = false; }
        if (string.IsNullOrEmpty(Password) || Password.Length < 6) { PasswordError = "Le mot de passe doit contenir au moins 6 caractères."; valid = false; }
        if (Password != ConfirmPassword) { ConfirmError = "Le mot de passe ne correspond pas"; valid = false; }
        if (!valid) return;

        IsBusy = true;
        var project = string.IsNullOrWhiteSpace(ProjectName) ? "Mon projet" : ProjectName;
        var (ok, error) = await _auth.RegisterAsync(FullName, project, Email, Password);
        IsBusy = false;

        if (!ok) { GeneralError = error ?? "Inscription impossible."; return; }
        await Shell.Current.GoToAsync("//main");
    }

    [RelayCommand]
    private Task BackToLoginAsync() => Shell.Current.GoToAsync("..");
}
