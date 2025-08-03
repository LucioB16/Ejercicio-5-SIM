using System.Windows;
using SimulacionCmi.ViewModels;

namespace SimulacionCmi;

/// <summary>
/// Ventana principal de la aplicación.
/// </summary>
public partial class MainWindow : Window
{
    private readonly CmiViewModel _vm = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
    }

    private void Simular_Click(object sender, RoutedEventArgs e)
    {
        _vm.EjecutarSimulacion();
        var ventana = new VentanaResultados
        {
            DataContext = new ResultadosViewModel(_vm.Vectores)
        };
        ventana.Show();
    }
}
