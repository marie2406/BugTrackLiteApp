using BugTrackLite.Helpers;
using SQLite;

namespace BugTrackLite.Models;

// Niveaux de priorité (Haute / Moyenne / Faible dans les maquettes).
public enum TicketPriority { Faible = 0, Moyenne = 1, Haute = 2 }

// Colonnes du tableau Kanban : À faire / En cours / Terminé.
public enum TicketStatus { Todo = 0, InProgress = 1, Done = 2 }

// Un ticket (tâche) stocké dans la base SQLite.
public class Ticket
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public int UserId { get; set; }          // propriétaire du ticket
    public string Title { get; set; } = "";
    public string Note { get; set; } = "";             // description / note optionnelle
    public TicketPriority Priority { get; set; } = TicketPriority.Moyenne;
    public TicketStatus Status { get; set; } = TicketStatus.Todo;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? StartedAt { get; set; }           // date de passage en "En cours"
    public DateTime? CompletedAt { get; set; }         // date de passage en "Terminé"
    public bool IsArchived { get; set; }               // true = retiré du tableau

    // ----- Propriétés calculées pour l'affichage (non enregistrées en base) -----

    [Ignore] public string Code => $"BT-{Id:000}";
    [Ignore] public string PriorityText => Priority.ToString();
    [Ignore] public string StatusText => StatusLabel(Status);

    [Ignore]
    public Color PriorityColor => Priority switch
    {
        TicketPriority.Haute => Color.FromArgb("#E5484D"),    // rouge
        TicketPriority.Moyenne => Color.FromArgb("#F5A524"),  // orange
        _ => Color.FromArgb("#30A46C")                        // vert
    };

    // Texte d'ancienneté : "Créé il y a 2j", "Commencé il y a 1 h", "Terminé hier"
    [Ignore]
    public string AgeText => Status switch
    {
        TicketStatus.InProgress => $"Commencé {TimeText.Since(StartedAt ?? CreatedAt)}",
        TicketStatus.Done => $"Terminé {TimeText.Since(CompletedAt ?? CreatedAt)}",
        _ => $"Créé {TimeText.Since(CreatedAt)}"
    };

    // Quels boutons afficher sur la carte ?
    [Ignore] public bool ShowStart => Status == TicketStatus.Todo;
    [Ignore] public bool ShowBack => Status == TicketStatus.InProgress;
    [Ignore] public bool ShowFinish => Status == TicketStatus.InProgress;
    [Ignore] public bool ShowArchive => Status == TicketStatus.Done;

    public static string StatusLabel(TicketStatus s) => s switch
    {
        TicketStatus.Todo => "À faire",
        TicketStatus.InProgress => "En cours",
        _ => "Terminé"
    };
}
