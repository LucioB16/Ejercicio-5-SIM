using System;
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

        double[] xs = vm.Visitas;

        // Ventas acumuladas
        plotVentas.Plot.Add.Scatter(xs, vm.VentasAcumuladas);
        plotVentas.Plot.Title("Ventas acumuladas");
        plotVentas.Plot.Axes.SetLimitsX(vm.PrimerVisita, vm.UltimaVisita);
        plotVentas.Refresh();

        // Probabilidad acumulada de "definitivamente sí" en escala logarítmica
        double[] logProb = vm.ProbabilidadSi.Select(p => p > 0 ? Math.Log10(p) : double.NegativeInfinity).ToArray();
        plotProbabilidad.Plot.Add.Scatter(xs, logProb);
        plotProbabilidad.Plot.Title("P(Def. Sí)");
        plotProbabilidad.Plot.Axes.SetLimitsX(vm.PrimerVisita, vm.UltimaVisita);
        plotProbabilidad.Plot.Axes.SetLimitsY(-4, 0);
        plotProbabilidad.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
            new double[] { -4, -3, -2, -1, 0 },
            new string[] { "≈0", "0,001", "0,01", "0,1", "1" });
        plotProbabilidad.Refresh();
    }
}
