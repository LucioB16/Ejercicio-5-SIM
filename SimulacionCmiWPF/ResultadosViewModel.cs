using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SimulacionCmiCore;

namespace SimulacionCmiWPF;

/// <summary>
/// ViewModel para la ventana de resultados.
/// </summary>
public class ResultadosViewModel
{
    public IList<VectorEstado> Vectores { get; }
    public VectorEstado Ultimo { get; }

    // NUEVO: envoltorio enumerable para el DataGrid
    public IEnumerable<VectorEstado> UltimaFila =>
        Ultimo is null ? Array.Empty<VectorEstado>()
                       : new[] { Ultimo };

    public double ProbabilidadSi { get; }
    public int VentasTotales { get; }
    public int? VisitaObjetivo { get; }
    public double VisitasAnaliticas { get; }

    public ICommand MostrarGraficos { get; }

    public ResultadosViewModel((List<VectorEstado> Vectores, VectorEstado UltimoVector, double ProbabilidadSi,
        int VentasTotales, int? VisitaObjetivo) datos, double visitasAnaliticas)
    {
        Vectores = datos.Vectores;
        Ultimo = datos.UltimoVector;
        ProbabilidadSi = datos.ProbabilidadSi;
        VentasTotales = datos.VentasTotales;
        VisitaObjetivo = datos.VisitaObjetivo;
        VisitasAnaliticas = visitasAnaliticas;
        MostrarGraficos = new RelayCommand(_ => AbrirGraficos());
    }

    private void AbrirGraficos()
    {
        var vm = new GraficosViewModel(Vectores);
        var win = new VentanaGraficos { DataContext = vm };
        win.Owner = Application.Current.MainWindow;
        win.ShowDialog();
    }
}
