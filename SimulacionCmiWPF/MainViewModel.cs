using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SimulacionCmiCore;

namespace SimulacionCmiWPF;

/// <summary>
/// ViewModel principal de la aplicación.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private int _visitas = 40000;
    private int _desdeVisita = 1;
    private int _hastaVisita = 100;
    private double _probRecuerda = 0.35;
    private double _probCompraDudoso = 0.60;
    private int _ventasObjetivo = 10000;
    private int? _semillaRng;

    /// <summary>Total de visitas a simular.</summary>
    public int VisitasASimular
    {
        get => _visitas;
        set { _visitas = value; OnPropertyChanged(nameof(VisitasASimular)); }
    }

    public int DesdeVisita
    {
        get => _desdeVisita;
        set { _desdeVisita = value; OnPropertyChanged(nameof(DesdeVisita)); }
    }

    public int HastaVisita
    {
        get => _hastaVisita;
        set { _hastaVisita = value; OnPropertyChanged(nameof(HastaVisita)); }
    }

    public double ProbabilidadRecuerda
    {
        get => _probRecuerda;
        set { _probRecuerda = value; OnPropertyChanged(nameof(ProbabilidadRecuerda)); }
    }

    public double ProbabilidadDudosoCompra
    {
        get => _probCompraDudoso;
        set { _probCompraDudoso = value; OnPropertyChanged(nameof(ProbabilidadDudosoCompra)); }
    }

    /// <summary>Cantidad de ventas a alcanzar.</summary>
    public int VentasObjetivo
    {
        get => _ventasObjetivo;
        set { _ventasObjetivo = value; OnPropertyChanged(nameof(VentasObjetivo)); }
    }

    /// <summary>Semilla opcional del generador aleatorio.</summary>
    public int? SemillaRng
    {
        get => _semillaRng;
        set { _semillaRng = value; OnPropertyChanged(nameof(SemillaRng)); }
    }

    public ObservableCollection<ProbabilidadesFila> TablaRecuerda { get; } = new([
        new ProbabilidadesFila { No = 0.55, Dudoso = 0.15, Si = 0.30 }
    ]);

    public ObservableCollection<ProbabilidadesFila> TablaNoRecuerda { get; } = new([
        new ProbabilidadesFila { No = 0.70, Dudoso = 0.25, Si = 0.05 }
    ]);

    public ICommand EjecutarSimulacion { get; }
    public ICommand MostrarEnunciado { get; }

    public MainViewModel()
    {
        EjecutarSimulacion = new RelayCommand(_ => Simular(), _ => Validar(out _));
        MostrarEnunciado = new RelayCommand(_ => MostrarReadme());
    }

    private void Simular()
    {
        if (!Validar(out string mensaje))
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        try
        {
            var motor = new MotorCmi(ProbabilidadRecuerda,
                [TablaRecuerda[0].No, TablaRecuerda[0].Dudoso, TablaRecuerda[0].Si],
                [TablaNoRecuerda[0].No, TablaNoRecuerda[0].Dudoso, TablaNoRecuerda[0].Si],
                ProbabilidadDudosoCompra, VentasObjetivo);
            var res = motor.Simular(VisitasASimular, DesdeVisita, HastaVisita, SemillaRng);
            var vm = new ResultadosViewModel(res, motor.CalcularVisitasAnaliticas());
            var win = new VentanaResultados { DataContext = vm };
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }
        catch
        {
            MessageBox.Show("Error al ejecutar la simulación", "Simulación", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MostrarReadme()
    {
        var ventana = new VentanaEnunciado();
        ventana.Owner = Application.Current.MainWindow;
        ventana.ShowDialog();
    }

    private bool Validar(out string mensaje)
    {
        List<string> errores = [];
        if (VisitasASimular < 10)
            errores.Add("Visitas a simular debe ser al menos 10.");
        if (DesdeVisita < 1)
            errores.Add("Desde visita debe ser mayor o igual a 1.");
        if (HastaVisita < DesdeVisita)
            errores.Add("Hasta visita debe ser mayor o igual a Desde visita.");
        if (HastaVisita > VisitasASimular)
            errores.Add("Hasta visita no puede superar las visitas a simular.");
        if (HastaVisita - DesdeVisita > 10000)
            errores.Add("El rango de visitas para la tabla no puede exceder 10 000.");
        if (ProbabilidadRecuerda <= 0 || ProbabilidadRecuerda >= 1)
            errores.Add("La probabilidad de recordar debe estar entre 0 y 1.");
        if (ProbabilidadDudosoCompra <= 0 || ProbabilidadDudosoCompra >= 1)
            errores.Add("La probabilidad de compra para dudosos debe estar entre 0 y 1.");
        if (VentasObjetivo <= 0)
            errores.Add("Las ventas objetivo deben ser mayores que 0.");
        if (SemillaRng is < int.MinValue or > int.MaxValue)
            errores.Add("La semilla RNG debe estar dentro del rango de 32 bits.");
        if (!ValidarFila(TablaRecuerda[0], out string errRec))
            errores.Add($"Tabla 'Recuerda': {errRec}");
        if (!ValidarFila(TablaNoRecuerda[0], out string errNoRec))
            errores.Add($"Tabla 'No recuerda': {errNoRec}");
        mensaje = string.Join("\n", errores);
        return errores.Count == 0;
    }

    private static bool ValidarFila(ProbabilidadesFila fila, out string error)
    {
        string eNo = fila[nameof(ProbabilidadesFila.No)];
        string eDu = fila[nameof(ProbabilidadesFila.Dudoso)];
        string eSi = fila[nameof(ProbabilidadesFila.Si)];
        if (!string.IsNullOrEmpty(eNo)) { error = eNo; return false; }
        if (!string.IsNullOrEmpty(eDu)) { error = eDu; return false; }
        if (!string.IsNullOrEmpty(eSi)) { error = eSi; return false; }
        error = string.Empty; return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        CommandManager.InvalidateRequerySuggested();
    }
}
