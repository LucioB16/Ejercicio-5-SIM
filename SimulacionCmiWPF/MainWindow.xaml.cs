using System.Windows;

namespace SimulacionCmiWPF;

/// <summary>
/// Ventana principal minimal para ejecutar la simulación.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
