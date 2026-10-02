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

        // 1. Capa de Red y Servicios (Singleton: instancia única compartida)
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<IPostService, PostService>();

        // 2. Capa de Presentación - ViewModels (Transient: instancia limpia por pantalla)
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<DetalleViewModel>();

        // 3. Capa de Presentación - Vistas (Transient: ciclo de vida vinculado a la navegación)
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DetallePage>();

        return builder.Build();
    }
}