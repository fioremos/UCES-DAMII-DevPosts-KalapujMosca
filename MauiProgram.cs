using DevPostsApp.Services;
using DevPostsApp.ViewModels;
using DevPostsApp.Views;

namespace DevPostsApp;

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

        // 1. Registro de HttpClient y Servicios (Singleton)
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<IPostService, PostService>();

        // 2. Registro de ViewModels (Transient: arranque limpio)
        builder.Services.AddTransient<MainViewModel>();

        // 3. Registro de Páginas (Transient)
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}