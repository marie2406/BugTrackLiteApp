using SQLite;

namespace BugTrackLite.Models;

// Une ligne de l'historique d'un ticket ("20:12 Ticket déplacé vers En cours").
public class TicketEvent
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public int TicketId { get; set; }
    public DateTime At { get; set; } = DateTime.Now;
    public string Text { get; set; } = "";
}
