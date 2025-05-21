using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using Japanese_App.Data;
using Japanese_App.PageModels;
using Japanese_App.Pages;
using System.IO;
using Syncfusion.Maui.Core.Hosting;

namespace Japanese_App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
            builder.Logging.AddDebug();

            builder.Services.AddTransient<NuevaActividadPageModel>();

#endif

            // Ruta a la base de datos SQLite
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "japaneseapp.db3");
            builder.Services.AddSingleton(new DatabaseService(dbPath));

            // Servicios y repositorios
            builder.Services.AddSingleton<ProjectRepository>();
            builder.Services.AddSingleton<TaskRepository>();
            builder.Services.AddSingleton<CategoryRepository>();
            builder.Services.AddSingleton<TagRepository>();
            builder.Services.AddSingleton<SeedDataService>();
            builder.Services.AddSingleton<ModalErrorHandler>();

            // ViewModels / PageModels
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<ProjectListPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();

            // Páginas - transient para nuevas instancias
            builder.Services.AddTransient<NuevaActividadPage>();


            // Registro de rutas Shell para navegación

            Routing.RegisterRoute("actividad", typeof(NuevaActividadPage));

            return builder.Build();
        }
    }
}
