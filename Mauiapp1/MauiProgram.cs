using Microsoft.Extensions.Logging;
using Mauiapp1.Services;
using Mauiapp1.ViewModels;

namespace Mauiapp1
{
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

            // Register services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            // Register view models
            builder.Services.AddTransient<LoginViewModel>();

            // Register pages
            builder.Services.AddTransient<LoginPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}