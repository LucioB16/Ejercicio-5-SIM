using System.Collections.Generic;
using System.Linq;
using SimulacionCmiCore;

namespace SimulacionCmiWPF;

/// <summary>
/// ViewModel para la ventana de gráficos.
/// </summary>
public class GraficosViewModel
{
    /// <summary>Número de visita para cada punto.</summary>
    public double[] Visitas { get; }

    /// <summary>Ventas acumuladas por visita.</summary>
    public double[] VentasAcumuladas { get; }

    /// <summary>Probabilidad acumulada de respuesta "definitivamente sí".</summary>
    public double[] ProbabilidadSi { get; }

    /// <summary>Primer índice de visita mostrado.</summary>
    public double PrimerVisita { get; }

    /// <summary>Último índice de visita mostrado.</summary>
    public double UltimaVisita { get; }

    public GraficosViewModel(IList<VectorEstado> vectores)
    {
        Visitas = vectores.Select(v => (double)v.Visita).ToArray();
        VentasAcumuladas = vectores.Select(v => (double)v.VentasAcum).ToArray();
        ProbabilidadSi = vectores.Select(v => v.ProbAcumSi).ToArray();
        PrimerVisita = Visitas.First();
        UltimaVisita = Visitas.Last();
    }
}
