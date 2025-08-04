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

        double[] xs = Enumerable.Range(vm.PrimeraVisita, vm.VentasAcumuladas.Length)
            .Select(i => (double)i).ToArray();

        plotVentas.Plot.Add.Scatter(xs, vm.VentasAcumuladas);
        plotVentas.Plot.Title("Ventas acumuladas");
        plotVentas.Plot.Axes.SetLimits(vm.PrimeraVisita, vm.UltimaVisita);
        plotVentas.Refresh();

        double[] logProb = vm.ProbabilidadSi
            .Select(p => Math.Log10(Math.Max(p, 1e-3)))
            .ToArray();
        plotProbabilidad.Plot.Add.Scatter(xs, logProb);
        plotProbabilidad.Plot.Title("P(Def. Sí)");
        plotProbabilidad.Plot.Axes.SetLimits(vm.PrimeraVisita, vm.UltimaVisita, -3, 0);

        var ticks = new ScottPlot.TickGenerators.NumericAutomatic
        {
            LabelFormatter = v => Math.Pow(10, v).ToString("0.###"),
            MinorTickGenerator = new ScottPlot.TickGenerators.EvenlySpacedMinorTickGenerator(0.05),
            MaxTickCount = 4
        };
        plotProbabilidad.Plot.Axes.Left.TickGenerator = ticks;
        plotProbabilidad.Refresh();
    }
}
