using BugTrackLite.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BugTrackLite.ViewModels;

// Écran "Bilan du projet" (statistiques).
public partial class StatsViewModel : ObservableObject
{
    private readonly TicketService _tickets;
    public StatsViewModel(TicketService tickets) => _tickets = tickets;

    [ObservableProperty] private int percent;
    [ObservableProperty] private double doneRatio;      // 0 à 1 pour les barres
    [ObservableProperty] private double todoRatio;
    [ObservableProperty] private double doingRatio;
    [ObservableProperty] private string todoText = "0%";
    [ObservableProperty] private string doingText = "0%";
    [ObservableProperty] private string doneText = "0%";
    [ObservableProperty] private string rhythmText = "0%";
    [ObservableProperty] private string rhythmDetail = "";

    [RelayCommand]
    private async Task LoadAsync()
    {
        var s = await _tickets.GetStatsAsync();
        Percent = s.Percent;
        TodoRatio = s.Ratio(s.Todo); DoingRatio = s.Ratio(s.Doing); DoneRatio = s.Ratio(s.Done);
        TodoText = $"{Math.Round(TodoRatio * 100)}%";
        DoingText = $"{Math.Round(DoingRatio * 100)}%";
        DoneText = $"{Math.Round(DoneRatio * 100)}%";

        var w = await _tickets.GetWeeklyStatsAsync();
        RhythmText = $"{(w.ChangePercent >= 0 ? "+" : "")}{w.ChangePercent}%";
        RhythmDetail = $"{w.ThisWeek} tâche(s) terminée(s) sur 7 jours, comparé à la semaine précédente";
    }

    [RelayCommand]
    private Task CloseAsync() => Shell.Current.GoToAsync("..");
}
