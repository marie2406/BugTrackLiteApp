using BugTrackLite.Models;
using SQLite;

namespace BugTrackLite.Services;

// Jeu de tickets d'exemple (inspiré des maquettes) pour qu'un nouveau compte ne soit pas vide.
public static class DemoData
{
    public static async Task SeedAsync(SQLiteAsyncConnection conn, int userId)
    {
        var now = DateTime.Now;

        Ticket T(string title, TicketPriority p, TicketStatus s, DateTime created, DateTime? started = null, DateTime? done = null, string note = "")
            => new() { UserId = userId, Title = title, Priority = p, Status = s, CreatedAt = created, StartedAt = started, CompletedAt = done, Note = note };

        var tickets = new List<Ticket>
        {
            // --- À faire ---
            T("Corriger le crash au login", TicketPriority.Haute, TicketStatus.Todo, now.AddDays(-2), note: "L'application se ferme après un mot de passe erroné."),
            T("Tester TinyDB", TicketPriority.Moyenne, TicketStatus.Todo, now.AddDays(-3)),
            T("Revoir les maquettes", TicketPriority.Moyenne, TicketStatus.Todo, now.AddDays(-1)),
            T("Rédiger les micro-textes", TicketPriority.Faible, TicketStatus.Todo, now.AddDays(-5)),
            // --- En cours ---
            T("Déplacement entre les listes", TicketPriority.Haute, TicketStatus.InProgress, now.AddDays(-1), now.AddMinutes(-30)),
            T("Tester la navigation", TicketPriority.Moyenne, TicketStatus.InProgress, now.AddDays(-1), now.AddHours(-1)),
            T("Préparer le modèle Task", TicketPriority.Moyenne, TicketStatus.InProgress, now.AddDays(-2), now.AddDays(-1)),
            // --- Terminé (dont un de la semaine précédente pour calculer le rythme) ---
            T("Maquette de l'accueil", TicketPriority.Moyenne, TicketStatus.Done, now.AddDays(-4), now.AddDays(-3), now.AddDays(-1)),
            T("Composant priorité", TicketPriority.Haute, TicketStatus.Done, now.AddDays(-4), now.AddDays(-3), now.AddDays(-1)),
            T("Structure du tableau", TicketPriority.Moyenne, TicketStatus.Done, now.AddDays(-6), now.AddDays(-5), now.AddDays(-3)),
            T("Navigation principale", TicketPriority.Moyenne, TicketStatus.Done, now.AddDays(-12), now.AddDays(-11), now.AddDays(-9)),
        };

        await conn.InsertAllAsync(tickets);   // remplit aussi les Id générés

        foreach (var t in tickets)
            await conn.InsertAsync(new TicketEvent { TicketId = t.Id, At = t.CreatedAt, Text = "Ticket créé" });
    }
}
