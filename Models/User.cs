using SQLite;

namespace BugTrackLite.Models;

// Compte utilisateur créé sur l'écran "Inscription".
public class User
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Unique] public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public string ProjectName { get; set; } = "";   // ex. "TP Réseau S5"
    public string PasswordHash { get; set; } = "";  // jamais le mot de passe en clair
    public string Salt { get; set; } = "";          // "sel" aléatoire pour le hachage
}
