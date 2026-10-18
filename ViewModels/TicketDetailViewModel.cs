using System.Collections.ObjectModel;
using BugTrackLite.Helpers;
using BugTrackLite.Models;
using BugTrackLite.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BugTrackLite.ViewModels;

// Écran "Détail du ticket". L'id arrive dans l'URL : "detail?id=5".
[QueryProperty(nameof(TicketId), "id")]
public partial class TicketDetailViewModel : ObservableObject
{
    private readonly TicketService _tickets;
    private readonly SettingsService _settings;

    public TicketDetailViewModel(TicketService tickets, SettingsService settings)
    {
        _tickets = tickets; _settings = settings;
    }

    [ObservableProperty] private int ticketId;
    [ObservableProperty] private Ticket? ticket;
    [ObservableProperty] private string createdLabel = "";
    [ObservableProperty] private bool canStart;     // boutons d'action visibles selon l'état
    [ObservableProperty] private bool canBack;
    [ObservableProperty] private bool canFinish;

    public ObservableCollection<string> History { get; } = new();

    // Dès que l'id est reçu, on charge le ticket.
    partial void OnTicketIdChanged(int value) => _ = LoadAsync();

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (TicketId <= 0) return;
        var t = await _tickets.GetAsync(TicketId);
        if (t == null) return;

        Ticket = t;
        CreatedLabel = $"{t.Code} · Créé {TimeText.Day(t.CreatedAt)}";
        CanStart = t.Status == TicketStatus.Todo;
        CanBack = t.Status == TicketStatus.InProgress;
        CanFinish = t.Status == TicketStatus.InProgress;

        History.Clear();
        foreach (var e in await _tickets.GetEventsAsync(t.Id))
        {
            var when = e.At.Date == DateTime.Today ? $"{e.At:HH:mm}" : $"{e.At:dd/MM HH:mm}";
            History.Add($"{when}  {e.Text}");
        }
    }

    private async Task MoveAsync(TicketStatus target)
    {
        if (Ticket == null) return;
        var (ok, error) = await _tickets.MoveAsync(Ticket, target);
        if (!ok) { await Dialogs.AlertAsync("Action impossible", error ?? ""); return; }
        await LoadAsync();
    }

    [RelayCommand] private Task StartAsync() => MoveAsync(TicketStatus.InProgress);
    [RelayCommand] private Task BackToTodoAsync() => MoveAsync(TicketStatus.Todo);
    [RelayCommand] private Task FinishAsync() => MoveAsync(TicketStatus.Done);

    [RelayCommand]
    private async Task ArchiveAsync()
    {
        if (Ticket == null) return;
        if (!await Dialogs.ConfirmArchiveAsync(Ticket, _settings)) return;
        await _tickets.ArchiveAsync(Ticket);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private Task CloseAsync() => Shell.Current.GoToAsync("..");
}
