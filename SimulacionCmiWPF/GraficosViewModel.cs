using System.Collections.Generic;
using System.Linq;
using SimulacionCmiCore;

namespace SimulacionCmiWPF;

/// <summary>
/// ViewModel para la ventana de gráficos.
/// </summary>
public class GraficosViewModel
{
    public double[] VentasAcumuladas { get; }
    public double[] ProbabilidadSi { get; }
    public int PrimeraVisita { get; }
    public int UltimaVisita { get; }

    public GraficosViewModel(IList<VectorEstado> vectores)
    {
        VentasAcumuladas = vectores.Select(v => (double)v.VentasAcum).ToArray();
        ProbabilidadSi = vectores.Select(v => v.ProbAcumSi).ToArray();
        if (vectores.Count > 0)
        {
            PrimeraVisita = vectores[0].Visita;
            UltimaVisita = vectores[^1].Visita;
        }
    }
}
