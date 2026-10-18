using BugTrackLite.Models;

namespace BugTrackLite.Services;

// Résumé pour le tableau de bord et les statistiques.
public record BoardStats(int Todo, int Doing, int Done)
{
    public int Total => Todo + Doing + Done;
    public int Percent => Total == 0 ? 0 : (int)Math.Round(100.0 * Done / Total);
    public double Ratio(int n) => Total == 0 ? 0 : (double)n / Total;
}

// Résultat hebdomadaire : tickets terminés sur 7 jours et variation vs la semaine précédente.
public record WeeklyStats(int ThisWeek, int ChangePercent);

// Toute la logique métier des tickets (création, déplacement, archivage, recherche, stats).
public class TicketService
{
    private readonly DatabaseService _db;
    private readonly AuthService _auth;
    private readonly SettingsService _settings;

    public TicketService(DatabaseService db, AuthService auth, SettingsService settings)
    {
        _db = db; _auth = auth; _settings = settings;
    }

    // ---------- Lecture ----------

    // Tous les tickets de l'utilisateur connecté (les volumes sont petits : on filtre en mémoire).
    private async Task<List<Ticket>> LoadAllAsync(bool includeArchived = false)
    {
        var conn = await _db.GetAsync();
        var uid = _auth.CurrentUserId;
        var list = await conn.Table<Ticket>().Where(t => t.UserId == uid).ToListAsync();
        return includeArchived ? list : list.Where(t => !t.IsArchived).ToList();
    }

    // Tri choisi dans les Réglages : par priorité (puis date) ou par date.
    private IEnumerable<Ticket> Sort(IEnumerable<Ticket> tickets)
        => _settings.DefaultSort == "Date"
            ? tickets.OrderByDescending(t => t.CreatedAt)
            : tickets.OrderByDescending(t => (int)t.Priority).ThenByDescending(t => t.CreatedAt);

    public async Task<List<Ticket>> GetByStatusAsync(TicketStatus status)
        => Sort((await LoadAllAsync()).Where(t => t.Status == status)).ToList();

    public async Task<Ticket?> GetAsync(int id)
    {
        var conn = await _db.GetAsync();
        var t = await conn.FindAsync<Ticket>(id);
        return t != null && t.UserId == _auth.CurrentUserId ? t : null;
    }

    // "Priorités du jour" : tickets non terminés, les plus urgents d'abord.
    public async Task<List<Ticket>> GetPriorityTicketsAsync(int count)
        => (await LoadAllAsync())
            .Where(t => t.Status != TicketStatus.Done)
            .OrderByDescending(t => (int)t.Priority).ThenBy(t => t.CreatedAt)
            .Take(count).ToList();

    public async Task<List<TicketEvent>> GetEventsAsync(int ticketId)
    {
        var conn = await _db.GetAsync();
        return await conn.Table<TicketEvent>().Where(e => e.TicketId == ticketId)
                         .OrderByDescending(e => e.At).ToListAsync();
    }

    // ---------- Écriture ----------

    private string LimitMessage =>
        $"Limite « En cours » atteinte ({_settings.InProgressLimit}). Terminez ou remettez à faire une tâche avant d'en démarrer une autre.";

    private async Task<bool> IsLimitReachedAsync()
        => (await GetByStatusAsync(TicketStatus.InProgress)).Count >= _settings.InProgressLimit;

    private async Task LogAsync(int ticketId, string text)
    {
        var conn = await _db.GetAsync();
        await conn.InsertAsync(new TicketEvent { TicketId = ticketId, Text = text });
    }

    public async Task<(bool Ok, string? Error)> CreateAsync(string title, string note, TicketPriority priority, TicketStatus status)
    {
        if (status == TicketStatus.InProgress && await IsLimitReachedAsync())
            return (false, LimitMessage);

        var conn = await _db.GetAsync();
        var ticket = new Ticket
        {
            UserId = _auth.CurrentUserId,
            Title = title.Trim(),
            Note = note.Trim(),
            Priority = priority,
            Status = status,
            CreatedAt = DateTime.Now,
            StartedAt = status == TicketStatus.InProgress ? DateTime.Now : null
        };
        await conn.InsertAsync(ticket);
        await LogAsync(ticket.Id, "Ticket créé");
        if (status == TicketStatus.InProgress) await LogAsync(ticket.Id, "Ticket déplacé vers En cours");
        return (true, null);
    }

    // Déplace un ticket d'une colonne à l'autre en respectant la limite "En cours".
    public async Task<(bool Ok, string? Error)> MoveAsync(Ticket ticket, TicketStatus target)
    {
        if (target == TicketStatus.InProgress && ticket.Status != TicketStatus.InProgress && await IsLimitReachedAsync())
            return (false, LimitMessage);

        ticket.Status = target;
        switch (target)
        {
            case TicketStatus.InProgress: ticket.StartedAt = DateTime.Now; ticket.CompletedAt = null; break;
            case TicketStatus.Done: ticket.CompletedAt = DateTime.Now; break;
            default: ticket.StartedAt = null; ticket.CompletedAt = null; break;
        }

        var conn = await _db.GetAsync();
        await conn.UpdateAsync(ticket);
        await LogAsync(ticket.Id, $"Ticket déplacé vers {Ticket.StatusLabel(target)}");
        return (true, null);
    }

    // Archivage : le ticket disparaît du tableau mais reste en base (sert encore aux statistiques hebdo).
    public async Task ArchiveAsync(Ticket ticket)
    {
        ticket.IsArchived = true;
        var conn = await _db.GetAsync();
        await conn.UpdateAsync(ticket);
        await LogAsync(ticket.Id, "Ticket archivé");
    }

    // ---------- Recherche & statistiques ----------

    public async Task<List<Ticket>> SearchAsync(string? text, TicketStatus? status, TicketPriority? priority)
    {
        IEnumerable<Ticket> q = await LoadAllAsync();
        if (!string.IsNullOrWhiteSpace(text))
            q = q.Where(t => t.Title.Contains(text, StringComparison.OrdinalIgnoreCase)
                          || t.Note.Contains(text, StringComparison.OrdinalIgnoreCase));
        if (status != null) q = q.Where(t => t.Status == status);
        if (priority != null) q = q.Where(t => t.Priority == priority);
        return Sort(q).ToList();
    }

    public async Task<BoardStats> GetStatsAsync()
    {
        var all = await LoadAllAsync();
        return new BoardStats(
            all.Count(t => t.Status == TicketStatus.Todo),
            all.Count(t => t.Status == TicketStatus.InProgress),
            all.Count(t => t.Status == TicketStatus.Done));
    }

    // "Rythme" : tickets terminés les 7 derniers jours comparés aux 7 jours d'avant.
    public async Task<WeeklyStats> GetWeeklyStatsAsync()
    {
        var done = (await LoadAllAsync(includeArchived: true))
            .Where(t => t.Status == TicketStatus.Done && t.CompletedAt != null).ToList();
        var now = DateTime.Now;
        int current = done.Count(t => t.CompletedAt >= now.AddDays(-7));
        int previous = done.Count(t => t.CompletedAt < now.AddDays(-7) && t.CompletedAt >= now.AddDays(-14));
        int change = previous == 0 ? (current > 0 ? 100 : 0) : (int)Math.Round(100.0 * (current - previous) / previous);
        return new WeeklyStats(current, change);
    }
}
