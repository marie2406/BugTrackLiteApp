using BugTrackLite.ViewModels;

namespace BugTrackLite.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;   // lie la page à son ViewModel
    }
}
