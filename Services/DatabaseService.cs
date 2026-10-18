using BugTrackLite.Models;
using SQLite;

namespace BugTrackLite.Services;

// Ouvre (et crée si besoin) la base SQLite locale. Une seule connexion est partagée.
public class DatabaseService
{
    private SQLiteAsyncConnection? _conn;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<SQLiteAsyncConnection> GetAsync()
    {
        if (_conn != null) return _conn;

        await _lock.WaitAsync();   // évite deux initialisations simultanées
        try
        {
            if (_conn == null)
            {
                var path = Path.Combine(FileSystem.AppDataDirectory, "bugtracklite.db3");
                var conn = new SQLiteAsyncConnection(path);
                await conn.CreateTableAsync<User>();
                await conn.CreateTableAsync<Ticket>();
                await conn.CreateTableAsync<TicketEvent>();
                _conn = conn;
            }
        }
        finally { _lock.Release(); }

        return _conn!;
    }
}
