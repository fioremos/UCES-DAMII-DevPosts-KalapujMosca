using DevPostsApp.ViewModels;

namespace DevPostsApp.Views;

public partial class DetallePage : ContentPage
{
    public DetallePage(DetalleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}