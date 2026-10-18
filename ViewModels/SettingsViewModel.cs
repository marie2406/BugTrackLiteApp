using BugTrackLite.Helpers;
using BugTrackLite.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BugTrackLite.ViewModels;

// Écran "Réglages". Chaque changement est enregistré tout de suite dans SettingsService.
public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settings;
    private readonly AuthService _auth;

    public List<string> Sorts { get; } = new() { "Priorité", "Date" };

    [ObservableProperty] private string fullName = "";
    [ObservableProperty] private string projectName = "";
    [ObservableProperty] private string initials = "";
    [ObservableProperty] private bool showDone;
    [ObservableProperty] private int sortIndex;
    [ObservableProperty] private bool dailyReminder;
    [ObservableProperty] private double inProgressLimit;   // double car le Stepper utilise des double
    [ObservableProperty] private bool confirmArchive;

    public SettingsViewModel(SettingsService settings, AuthService auth)
    {
        _settings = settings; _auth = auth;
        // Valeurs de départ lues depuis les préférences.
        ShowDone = settings.ShowDone;
        SortIndex = Math.Max(0, Sorts.IndexOf(settings.DefaultSort));
        DailyReminder = settings.DailyReminder;
        InProgressLimit = settings.InProgressLimit;
        ConfirmArchive = settings.ConfirmArchive;
    }

    partial void OnShowDoneChanged(bool value) => _settings.ShowDone = value;
    partial void OnSortIndexChanged(int value) { if (value >= 0) _settings.DefaultSort = Sorts[value]; }
    partial void OnDailyReminderChanged(bool value) => _settings.DailyReminder = value;
    partial void OnInProgressLimitChanged(double value) => _settings.InProgressLimit = (int)value;
    partial void OnConfirmArchiveChanged(bool value) => _settings.ConfirmArchive = value;

    [RelayCommand]
    private async Task LoadAsync()
    {
        var user = await _auth.GetCurrentUserAsync();
        FullName = user?.FullName ?? "";
        ProjectName = user?.ProjectName ?? "";
        Initials = TimeText.Initials(user?.FullName);
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _auth.Logout();
        await Shell.Current.GoToAsync("//login");
    }
}
