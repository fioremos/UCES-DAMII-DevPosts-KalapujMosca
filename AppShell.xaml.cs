using DevPostsApp.Views;

namespace DevPostsApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        // Registro de ruta para navegación contextual hacia el detalle
        Routing.RegisterRoute(nameof(DetallePage), typeof(DetallePage));
    }
}
