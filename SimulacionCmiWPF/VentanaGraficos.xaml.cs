using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ScottPlot;
using SimulacionCmiCore;

namespace SimulacionCmiWPF;

/// <summary>
/// Ventana que muestra los gráficos de la simulación.
/// </summary>
public partial class VentanaGraficos : Window
{
    public VentanaGraficos(List<VectorEstado> vectores)
    {
        InitializeComponent();
        double[] xs = vectores.Select(v => (double)v.Visita).ToArray();
        double[] prob = vectores.Select(v => v.ProbAcumSi).ToArray();
        double[] ventas = vectores.Select(v => (double)v.VentasAcum).ToArray();
        plot.Plot.Add.Scatter(xs, prob); // P(Def. Sí)
        plot.Plot.Add.Scatter(xs, ventas); // Ventas
        plot.Plot.Title("Evolución");
        plot.Refresh();
    }
}
