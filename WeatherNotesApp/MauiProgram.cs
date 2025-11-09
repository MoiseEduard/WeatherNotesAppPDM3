using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WeatherNotesApp.Services;
using Microcharts.Maui;

namespace WeatherNotesApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMicrocharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            builder.Services.AddSingleton<DatabaseService>(s => new DatabaseService(dbPath));
            builder.Services.AddSingleton<IAlertService, MauiAlertService>();

            return builder.Build();
        }
    }
}
