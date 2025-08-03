using System.Collections.Generic;
using SimulacionCmi.Core;

namespace SimulacionCmi.ViewModels;

/// <summary>
/// ViewModel para la ventana de resultados.
/// </summary>
public class ResultadosViewModel
{
    /// <summary>Lista de vectores de estado a mostrar.</summary>
    public IReadOnlyList<VectorEstado> Vectores { get; }

    public ResultadosViewModel(IReadOnlyList<VectorEstado> vectores)
    {
        Vectores = vectores;
    }
}
