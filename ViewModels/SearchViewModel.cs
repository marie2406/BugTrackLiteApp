using System.Collections.ObjectModel;
using BugTrackLite.Models;
using BugTrackLite.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BugTrackLite.ViewModels;

// Écran "Recherche & filtres".
public partial class SearchViewModel : ObservableObject
{
    private readonly TicketService _tickets;
    public SearchViewModel(TicketService tickets) => _tickets = tickets;

    public List<string> States { get; } = new() { "Tous", "À faire", "En cours", "Terminé" };
    public List<string> Priorities { get; } = new() { "Toutes", "Haute", "Moyenne", "Faible" };
    public ObservableCollection<Ticket> Results { get; } = new();

    [ObservableProperty] private string query = "";
    [ObservableProperty] private int stateIndex = 0;
    [ObservableProperty] private int priorityIndex = 0;
    [ObservableProperty] private string resultText = "";

    // Chaque changement de critère relance la recherche.
    partial void OnQueryChanged(string value) => _ = SearchAsync();
    partial void OnStateIndexChanged(int value) => _ = SearchAsync();
    partial void OnPriorityIndexChanged(int value) => _ = SearchAsync();

    [RelayCommand]
    private async Task SearchAsync()
    {
        TicketStatus? status = StateIndex switch
        {
            1 => TicketStatus.Todo, 2 => TicketStatus.InProgress, 3 => TicketStatus.Done, _ => null
        };
        TicketPriority? priority = PriorityIndex switch
        {
            1 => TicketPriority.Haute, 2 => TicketPriority.Moyenne, 3 => TicketPriority.Faible, _ => null
        };

        var list = await _tickets.SearchAsync(Query, status, priority);
        Results.Clear();
        foreach (var t in list) Results.Add(t);
        ResultText = list.Count == 1 ? "1 résultat" : $"{list.Count} résultats";
    }

    [RelayCommand]
    private void Reset()
    {
        Query = ""; StateIndex = 0; PriorityIndex = 0;
    }

    [RelayCommand]
    private Task OpenDetailAsync(Ticket ticket) => Shell.Current.GoToAsync($"detail?id={ticket.Id}");

    [RelayCommand]
    private Task CloseAsync() => Shell.Current.GoToAsync("..");
}
