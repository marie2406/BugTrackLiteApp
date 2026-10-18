namespace BugTrackLite.Services;

// Réglages de l'écran "Réglages", mémorisés avec Preferences (stockage clé/valeur du téléphone).
public class SettingsService
{
    // Afficher la liste des tâches terminées ?
    public bool ShowDone
    {
        get => Preferences.Default.Get(nameof(ShowDone), true);
        set => Preferences.Default.Set(nameof(ShowDone), value);
    }

    // Tri par défaut des listes : "Priorité" ou "Date".
    public string DefaultSort
    {
        get => Preferences.Default.Get(nameof(DefaultSort), "Priorité");
        set => Preferences.Default.Set(nameof(DefaultSort), value);
    }

    // Rappel quotidien (préférence enregistrée ; la notification n'est pas implémentée).
    public bool DailyReminder
    {
        get => Preferences.Default.Get(nameof(DailyReminder), false);
        set => Preferences.Default.Set(nameof(DailyReminder), value);
    }

    // Nombre maximum de tickets simultanément "En cours" (règle Kanban WIP).
    public int InProgressLimit
    {
        get => Preferences.Default.Get(nameof(InProgressLimit), 3);
        set => Preferences.Default.Set(nameof(InProgressLimit), value);
    }

    // Demander une confirmation avant d'archiver ?
    public bool ConfirmArchive
    {
        get => Preferences.Default.Get(nameof(ConfirmArchive), true);
        set => Preferences.Default.Set(nameof(ConfirmArchive), value);
    }
}
