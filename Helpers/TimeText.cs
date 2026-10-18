namespace BugTrackLite.Helpers;

// Petites fonctions pour afficher les dates comme dans les maquettes ("il y a 2j", "hier"...).
public static class TimeText
{
    // Exemple : "il y a 30 min", "il y a 3 h", "hier", "il y a 5j"
    public static string Since(DateTime date)
    {
        var d = DateTime.Now - date;
        if (d.TotalMinutes < 1) return "à l'instant";
        if (d.TotalMinutes < 60) return $"il y a {(int)d.TotalMinutes} min";
        if (d.TotalHours < 24) return $"il y a {(int)d.TotalHours} h";
        if (d.TotalHours < 48) return "hier";
        return $"il y a {(int)d.TotalDays}j";
    }

    // Exemple : "aujourd'hui", "hier", "le 12/10/2026"
    public static string Day(DateTime date)
    {
        if (date.Date == DateTime.Today) return "aujourd'hui";
        if (date.Date == DateTime.Today.AddDays(-1)) return "hier";
        return $"le {date:dd/MM/yyyy}";
    }

    // Initiales d'un nom complet : "Marie Essane" => "ME"
    public static string Initials(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return "?";
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Take(2).Select(p => char.ToUpper(p[0])));
    }
}
