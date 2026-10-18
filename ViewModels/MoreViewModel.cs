using CommunityToolkit.Mvvm.Input;

namespace BugTrackLite.ViewModels;

// Onglet "Plus" : raccourcis vers Nouveau ticket, Recherche, Statistiques et Réglages.
public partial class MoreViewModel
{
    [RelayCommand]
    private Task GoToAsync(string route) => Shell.Current.GoToAsync(route);
}
