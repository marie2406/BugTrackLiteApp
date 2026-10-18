using BugTrackLite.Services;
using BugTrackLite.ViewModels;
using BugTrackLite.Views;

namespace BugTrackLite;

// Point d'entrée de l'application : on déclare ici tous les services, ViewModels et pages
// afin que .NET MAUI les injecte automatiquement (injection de dépendances).
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // --- Services (une seule instance pour toute l'application) ---
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<SettingsService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<TicketService>();

        // --- ViewModels (une nouvelle instance par page) ---
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<TicketListViewModel>();
        builder.Services.AddTransient<MoreViewModel>();
        builder.Services.AddTransient<AddTicketViewModel>();
        builder.Services.AddTransient<TicketDetailViewModel>();

        // --- Pages ---
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<TodoPage>();
        builder.Services.AddTransient<InProgressPage>();
        builder.Services.AddTransient<DonePage>();
        builder.Services.AddTransient<MorePage>();
        builder.Services.AddTransient<AddTicketPage>();
        builder.Services.AddTransient<TicketDetailPage>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<StatsPage>();
        builder.Services.AddTransient<SettingsPage>();

        return builder.Build();
    }
}
