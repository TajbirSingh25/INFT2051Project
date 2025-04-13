using Microsoft.Extensions.Logging;
using Mauiapp1.Services;
using Mauiapp1.ViewModels;
using System.IO;
using Mauiapp1.Views;

namespace Mauiapp1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Set up database path
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");

        // Initialize chat database service (singleton instance)
        var chatDbService = ChatDatabaseService.Instance;

        // Register services
        builder.Services.AddSingleton<IDatabaseService>(s => new DatabaseService(dbPath));
        builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // Register ChatDatabaseService as a service
        builder.Services.AddSingleton(chatDbService);

        // Register view models
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();

        // Register pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<ProfilePage>();

        // Add ChatPage registration
        builder.Services.AddTransient<Views.ChatPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register DatabaseService with dependency injection
        builder.Services.AddSingleton<IDatabaseService>(serviceProvider =>
            new DatabaseService(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "products.db3")));  // Properly closed here

        return builder.Build();
    }
}