using System.Collections.Generic;
using System.Linq;
using SimulacionCmi.Core;

namespace SimulacionCmi.ViewModels;

/// <summary>
/// ViewModel que expone los datos para graficar.
/// </summary>
public class GraficosViewModel
{
    /// <summary>Valores de la probabilidad acumulada de "definitivamente sí".</summary>
    public IReadOnlyList<double> ProbabilidadesSi { get; }

    /// <summary>Valores de las ventas acumuladas.</summary>
    public IReadOnlyList<int> VentasAcumuladas { get; }

    public GraficosViewModel(IReadOnlyList<VectorEstado> vectores)
    {
        ProbabilidadesSi = vectores.Select(v => v.ProbAcumSi).ToList();
        VentasAcumuladas = vectores.Select(v => v.VentasAcumuladas).ToList();
    }
}
