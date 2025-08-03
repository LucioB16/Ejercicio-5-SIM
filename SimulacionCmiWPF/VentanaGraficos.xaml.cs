using System.Linq;
using System.Windows;
using ScottPlot;

namespace SimulacionCmiWPF;

/// <summary>
/// Ventana que muestra los gráficos de la simulación.
/// </summary>
public partial class VentanaGraficos : Window
{
    public VentanaGraficos()
    {
        InitializeComponent();
        Loaded += VentanaGraficos_Loaded;
    }

    private void VentanaGraficos_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not GraficosViewModel vm)
            return;

        double[] xs = Enumerable.Range(1, vm.VentasAcumuladas.Length).Select(i => (double)i).ToArray();
        plotVentas.Plot.Add.Scatter(xs, vm.VentasAcumuladas);
        plotVentas.Plot.Title("Ventas acumuladas");
        plotVentas.Refresh();

        plotProbabilidad.Plot.Add.Scatter(xs, vm.ProbabilidadSi);
        plotProbabilidad.Plot.Title("P(Def. Sí)");
        plotProbabilidad.Refresh();
    }
}
