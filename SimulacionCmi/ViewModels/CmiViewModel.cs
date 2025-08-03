using System.Collections.ObjectModel;
using System.ComponentModel;
using SimulacionCmi.Core;

namespace SimulacionCmi.ViewModels;

/// <summary>
/// ViewModel principal de la aplicación.
/// </summary>
public class CmiViewModel : INotifyPropertyChanged
{
    private int _visitas = 40000;
    private int _desde = 1;
    private int _hasta = 40000;
    private int _ventasObjetivo = 10000;
    private double _probCompraDudoso = 0.60;
    private double _probabilidadDefSi;
    private int? _visitaObjetivo;
    private double _visitasAnaliticas;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Cantidad total de visitas a simular.</summary>
    public int Visitas
    {
        get => _visitas;
        set { _visitas = value; Notificar(nameof(Visitas)); }
    }

    /// <summary>Índice inicial para mostrar.</summary>
    public int Desde
    {
        get => _desde;
        set { _desde = value; Notificar(nameof(Desde)); }
    }

    /// <summary>Índice final para mostrar.</summary>
    public int Hasta
    {
        get => _hasta;
        set { _hasta = value; Notificar(nameof(Hasta)); }
    }

    /// <summary>Cantidad de ventas objetivo.</summary>
    public int VentasObjetivo
    {
        get => _ventasObjetivo;
        set { _ventasObjetivo = value; Notificar(nameof(VentasObjetivo)); }
    }

    /// <summary>Probabilidad de que un dudoso compre.</summary>
    public double ProbCompraDudoso
    {
        get => _probCompraDudoso;
        set { _probCompraDudoso = value; Notificar(nameof(ProbCompraDudoso)); }
    }

    /// <summary>Probabilidad acumulada final de "definitivamente sí".</summary>
    public double ProbabilidadDefSi
    {
        get => _probabilidadDefSi;
        private set { _probabilidadDefSi = value; Notificar(nameof(ProbabilidadDefSi)); }
    }

    /// <summary>Visita en que se alcanzan las ventas objetivo.</summary>
    public int? VisitaObjetivo
    {
        get => _visitaObjetivo;
        private set { _visitaObjetivo = value; Notificar(nameof(VisitaObjetivo)); }
    }

    /// <summary>Cantidad de visitas analítica para alcanzar la meta.</summary>
    public double VisitasAnaliticas
    {
        get => _visitasAnaliticas;
        private set { _visitasAnaliticas = value; Notificar(nameof(VisitasAnaliticas)); }
    }

    /// <summary>VectorEstado dentro del rango indicado.</summary>
    public ObservableCollection<VectorEstado> Vectores { get; } = new();

    /// <summary>Ejecuta la simulación utilizando los parámetros actuales.</summary>
    public void EjecutarSimulacion()
    {
        var motor = new MotorCmi();
        var distRecuerda = new DistribucionRespuesta(0.55, 0.15, 0.30);
        var distNoRecuerda = new DistribucionRespuesta(0.70, 0.25, 0.05);
        double probRecuerda = 0.35;

        var resultado = motor.Simular(
            Visitas,
            probRecuerda,
            distRecuerda,
            distNoRecuerda,
            ProbCompraDudoso,
            VentasObjetivo,
            Desde,
            Hasta);

        Vectores.Clear();
        foreach (var v in resultado.Vectores)
        {
            Vectores.Add(v);
        }

        ProbabilidadDefSi = resultado.UltimoVector.ProbAcumSi;
        VisitaObjetivo = resultado.VisitaObjetivo;
        VisitasAnaliticas = MotorCmi.CalcularVisitasAnaliticas(probRecuerda, distRecuerda, distNoRecuerda, ProbCompraDudoso, VentasObjetivo);
    }

    private void Notificar(string nombre)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
}
