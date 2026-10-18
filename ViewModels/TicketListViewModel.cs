using System.Collections.ObjectModel;
using BugTrackLite.Helpers;
using BugTrackLite.Models;
using BugTrackLite.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BugTrackLite.ViewModels;

// ViewModel commun aux écrans À faire / En cours / Terminé.
// La page indique sa colonne via la propriété Status.
public partial class TicketListViewModel : ObservableObject
{
    private readonly TicketService _tickets;
    private readonly SettingsService _settings;

    public TicketListViewModel(TicketService tickets, SettingsService settings)
    {
        _tickets = tickets; _settings = settings;
    }

    public TicketStatus Status { get; set; }

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string subtitle = "";
    [ObservableProperty] private string bannerTitle = "";   // encadré "Focus" ou "Cette semaine"
    [ObservableProperty] private string bannerText = "";
    [ObservableProperty] private string emptyMessage = "Aucune tâche ici pour le moment.";
    [ObservableProperty] private string searchText = "";
    [ObservableProperty] private string selectedPeriod = "Tout";   // filtre de période (écran Terminé)

    public List<string> Periods { get; } = new() { "Tout", "Aujourd'hui", "7 derniers jours", "30 derniers jours" };
    public ObservableCollection<Ticket> Tickets { get; } = new();

    // Recharge la liste quand on tape dans la recherche ou qu'on change de période.
    partial void OnSearchTextChanged(string value) => _ = LoadAsync();
    partial void OnSelectedPeriodChanged(string value) => _ = LoadAsync();

    [RelayCommand]
    private async Task LoadAsync()
    {
        var list = await _tickets.GetByStatusAsync(Status);
        int total = list.Count;

        // Réglage "Afficher les terminés" désactivé => on masque la liste.
        if (Status == TicketStatus.Done && !_settings.ShowDone)
        {
            list.Clear();
            EmptyMessage = "L'affichage des tâches terminées est désactivé dans les Réglages.";
        }
        else EmptyMessage = "Aucune tâche ici pour le moment.";

        if (!string.IsNullOrWhiteSpace(SearchText))
            list = list.Where(t => t.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        if (Status == TicketStatus.Done)
        {
            var now = DateTime.Now;
            list = SelectedPeriod switch
            {
                "Aujourd'hui" => list.Where(t => t.CompletedAt?.Date == now.Date).ToList(),
                "7 derniers jours" => list.Where(t => t.CompletedAt >= now.AddDays(-7)).ToList(),
                "30 derniers jours" => list.Where(t => t.CompletedAt >= now.AddDays(-30)).ToList(),
                _ => list
            };
        }

        Tickets.Clear();
        foreach (var t in list) Tickets.Add(t);

        await UpdateHeaderAsync(total);
    }

    // Titres et encadrés propres à chaque colonne.
    private async Task UpdateHeaderAsync(int total)
    {
        BannerTitle = BannerText = "";
        switch (Status)
        {
            case TicketStatus.Todo:
                Title = "À faire"; Subtitle = $"{total} tâche(s) à démarrer";
                break;
            case TicketStatus.InProgress:
                Title = "En cours"; Subtitle = $"{total} tâche(s) active(s)";
                int urgent = Tickets.Count(t => t.Priority == TicketPriority.Haute);
                if (urgent > 0)
                {
                    BannerTitle = "Focus";
                    BannerText = $"Vous avez {urgent} tâche(s) prioritaire(s) à terminer aujourd'hui.";
                }
                break;
            default:
                Title = "Terminé"; Subtitle = $"{total} tâche(s) clôturée(s)";
                var w = await _tickets.GetWeeklyStatsAsync();
                BannerTitle = "Cette semaine";
                BannerText = $"{w.ThisWeek} tâche(s) livrée(s) · {(w.ChangePercent >= 0 ? "+" : "")}{w.ChangePercent}% vs. semaine précédente";
                break;
        }
    }

    // ----- Actions des cartes -----

    private async Task MoveAsync(Ticket ticket, TicketStatus target)
    {
        var (ok, error) = await _tickets.MoveAsync(ticket, target);
        if (!ok) await Dialogs.AlertAsync("Action impossible", error ?? "");
        await LoadAsync();
    }

    [RelayCommand] private Task StartAsync(Ticket t) => MoveAsync(t, TicketStatus.InProgress);
    [RelayCommand] private Task BackToTodoAsync(Ticket t) => MoveAsync(t, TicketStatus.Todo);
    [RelayCommand] private Task FinishAsync(Ticket t) => MoveAsync(t, TicketStatus.Done);

    [RelayCommand]
    private async Task ArchiveAsync(Ticket ticket)
    {
        if (!await Dialogs.ConfirmArchiveAsync(ticket, _settings)) return;
        await _tickets.ArchiveAsync(ticket);
        await LoadAsync();
    }

    [RelayCommand]
    private Task OpenDetailAsync(Ticket ticket) => Shell.Current.GoToAsync($"detail?id={ticket.Id}");
}
