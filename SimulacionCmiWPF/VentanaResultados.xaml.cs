using System.Collections.Generic;
using System.Windows;
using SimulacionCmiCore;

namespace SimulacionCmiWPF;

/// <summary>
/// Muestra la tabla de vectores de estado.
/// </summary>
public partial class VentanaResultados : Window
{
    public VentanaResultados(List<VectorEstado> vectores, double probSi, int? visitaObjetivo, int ventasTotales)
    {
        InitializeComponent();
        dgResultados.ItemsSource = vectores;
        string objetivo = visitaObjetivo.HasValue ? visitaObjetivo.Value.ToString() : "no alcanzado";
        lblResumen.Text = $"P(Def. Sí) = {probSi:F4} | Visita objetivo: {objetivo} | Ventas: {ventasTotales}";
    }
}
