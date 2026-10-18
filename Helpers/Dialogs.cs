using BugTrackLite.Models;
using BugTrackLite.Services;

namespace BugTrackLite.Helpers;

// Boîtes de dialogue communes à plusieurs écrans.
public static class Dialogs
{
    public static Task AlertAsync(string title, string message)
        => Shell.Current.DisplayAlert(title, message, "OK");

    // Confirmation d'archivage (peut être désactivée dans les Réglages : "Confirmer l'archivage").
    public static async Task<bool> ConfirmArchiveAsync(Ticket ticket, SettingsService settings)
    {
        if (!settings.ConfirmArchive) return true;
        return await Shell.Current.DisplayAlert(
            "Archiver ce ticket ?",
            $"Le ticket #{ticket.Code} sera retiré du tableau.\nCette action est définitive.",
            "Archiver", "Annuler");
    }
}
