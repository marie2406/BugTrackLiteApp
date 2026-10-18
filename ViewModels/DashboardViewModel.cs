using System.Collections.ObjectModel;
using BugTrackLite.Helpers;
using BugTrackLite.Models;
using BugTrackLite.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BugTrackLite.ViewModels;

// Écran "Accueil / Dashboard Projet".
public partial class DashboardViewModel : ObservableObject
{
    private readonly AuthService _auth;
    private readonly TicketService _tickets;

    public DashboardViewModel(AuthService auth, TicketService tickets)
    {
        _auth = auth; _tickets = tickets;
    }

    [ObservableProperty] private string projectName = "";
    [ObservableProperty] private string initials = "";
    [ObservableProperty] private string progressText = "0%";
    [ObservableProperty] private double progressValue;   // 0 à 1 pour la barre de progression
    [ObservableProperty] private int todoCount;
    [ObservableProperty] private int doingCount;
    [ObservableProperty] private int doneCount;

    public ObservableCollection<Ticket> PriorityTickets { get; } = new();

    // Appelée à chaque affichage de la page : recharge les chiffres.
    [RelayCommand]
    private async Task LoadAsync()
    {
        var user = await _auth.GetCurrentUserAsync();
        ProjectName = user?.ProjectName ?? "";
        Initials = TimeText.Initials(user?.FullName);

        var s = await _tickets.GetStatsAsync();
        TodoCount = s.Todo; DoingCount = s.Doing; DoneCount = s.Done;
        ProgressValue = s.Ratio(s.Done);
        ProgressText = $"{s.Percent}%";

        PriorityTickets.Clear();
        foreach (var t in await _tickets.GetPriorityTicketsAsync(3))
            PriorityTickets.Add(t);
    }

    [RelayCommand]
    private Task OpenDetailAsync(Ticket ticket) => Shell.Current.GoToAsync($"detail?id={ticket.Id}");

    // Navigation générique : "//main/todo" (onglet) ou "add", "search"... (page).
    [RelayCommand]
    private Task GoToAsync(string route) => Shell.Current.GoToAsync(route);
}
