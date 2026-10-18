using System.Security.Cryptography;
using System.Text.RegularExpressions;
using BugTrackLite.Models;

namespace BugTrackLite.Services;

// Inscription, connexion et déconnexion. Les mots de passe sont hachés (PBKDF2 + sel).
public class AuthService
{
    private const string KeyUserId = "current_user_id";

    // Mettre à false pour ne PAS créer de tickets d'exemple à l'inscription.
    public const bool SeedDemoData = true;

    // Compte du bouton "Continuer avec mon compte étudiant" (démo hors-ligne).
    public const string StudentEmail = "etudiant@bugtracklite.demo";
    public const string StudentPassword = "etudiant123";

    private readonly DatabaseService _db;
    public AuthService(DatabaseService db) => _db = db;

    // Id de l'utilisateur connecté (0 = personne). Conservé entre deux lancements.
    public int CurrentUserId => Preferences.Default.Get(KeyUserId, 0);
    public bool IsLoggedIn => CurrentUserId > 0;

    // Format d'e-mail simple : texte@texte.texte
    public static bool IsValidEmail(string? email)
        => !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    public async Task<(bool Ok, string? Error)> RegisterAsync(string fullName, string projectName, string email, string password)
    {
        var conn = await _db.GetAsync();
        email = email.Trim().ToLowerInvariant();

        var existing = await conn.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
        if (existing != null) return (false, "Un compte existe déjà avec cet e-mail.");

        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        var user = new User
        {
            Email = email,
            FullName = fullName.Trim(),
            ProjectName = projectName.Trim(),
            Salt = salt,
            PasswordHash = Hash(password, salt)
        };
        await conn.InsertAsync(user);

        if (SeedDemoData) await DemoData.SeedAsync(conn, user.Id);

        Preferences.Default.Set(KeyUserId, user.Id);   // connexion automatique
        return (true, null);
    }

    public async Task<(bool Ok, string? Error)> LoginAsync(string email, string password)
    {
        var conn = await _db.GetAsync();
        email = email.Trim().ToLowerInvariant();

        var user = await conn.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
        if (user == null || user.PasswordHash != Hash(password, user.Salt))
            return (false, "E-mail ou mot de passe incorrect.");

        Preferences.Default.Set(KeyUserId, user.Id);
        return (true, null);
    }

    // Connexion "compte étudiant" : on le crée au premier usage puis on s'y connecte.
    public async Task<(bool Ok, string? Error)> LoginStudentAsync()
    {
        var result = await LoginAsync(StudentEmail, StudentPassword);
        if (result.Ok) return result;
        return await RegisterAsync("Étudiant Démo", "TP Réseaux – S5", StudentEmail, StudentPassword);
    }

    public void Logout() => Preferences.Default.Remove(KeyUserId);

    public async Task<User?> GetCurrentUserAsync()
    {
        if (!IsLoggedIn) return null;
        var conn = await _db.GetAsync();
        return await conn.FindAsync<User>(CurrentUserId);
    }

    // Hachage PBKDF2 : on ne stocke jamais le mot de passe lui-même.
    private static string Hash(string password, string saltBase64)
    {
        var salt = Convert.FromBase64String(saltBase64);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(hash);
    }
}
