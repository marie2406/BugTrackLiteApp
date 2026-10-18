using BugTrackLite.Helpers;
using BugTrackLite.Models;
using BugTrackLite.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BugTrackLite.ViewModels;

// Écran "Nouveau ticket".
public partial class AddTicketViewModel : ObservableObject
{
    private readonly TicketService _tickets;
    public AddTicketViewModel(TicketService tickets) => _tickets = tickets;

    public List<string> Priorities { get; } = new() { "Haute", "Moyenne", "Faible" };
    public List<string> Columns { get; } = new() { "À faire", "En cours" };

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string note = "";
    [ObservableProperty] private string titleError = "";
    [ObservableProperty] private int priorityIndex = 1;   // Moyenne par défaut
    [ObservableProperty] private int columnIndex = 0;     // À faire par défaut

    [RelayCommand]
    private async Task SaveAsync()
    {
        TitleError = "";
        if (string.IsNullOrWhiteSpace(Title))
        {
            TitleError = "Ne laisse pas ce champ vide !";
            return;
        }

        var priority = PriorityIndex switch { 0 => TicketPriority.Haute, 2 => TicketPriority.Faible, _ => TicketPriority.Moyenne };
        var status = ColumnIndex == 1 ? TicketStatus.InProgress : TicketStatus.Todo;

        var (ok, error) = await _tickets.CreateAsync(Title, Note, priority, status);
        if (!ok) { await Dialogs.AlertAsync("Action impossible", error ?? ""); return; }

        Title = ""; Note = "";
        await Shell.Current.GoToAsync("..");   // retour à l'écran précédent
    }

    [RelayCommand]
    private Task CancelAsync() => Shell.Current.GoToAsync("..");
}
