using System.Windows;
using SimulacionCmi.ViewModels;

namespace SimulacionCmi;

/// <summary>
/// Ventana que muestra la tabla de resultados de la simulación.
/// </summary>
public partial class VentanaResultados : Window
{
    public VentanaResultados()
    {
        InitializeComponent();
    }

    private void VerGraficos_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ResultadosViewModel vm)
        {
            var ventana = new VentanaGraficos
            {
                DataContext = new GraficosViewModel(vm.Vectores)
            };
            ventana.Show();
        }
    }
}
