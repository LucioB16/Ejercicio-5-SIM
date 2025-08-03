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

    public int Visitas
    {
        get => _visitas;
        set { _visitas = value; OnPropertyChanged(nameof(Visitas)); }
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
        EjecutarSimulacion = new RelayCommand(_ => Simular());
        MostrarEnunciado = new RelayCommand(_ => MostrarReadme());
    }

    private void Simular()
    {
        try
        {
            var motor = new MotorCmi(ProbabilidadRecuerda,
                [TablaRecuerda[0].No, TablaRecuerda[0].Dudoso, TablaRecuerda[0].Si],
                [TablaNoRecuerda[0].No, TablaNoRecuerda[0].Dudoso, TablaNoRecuerda[0].Si],
                ProbabilidadDudosoCompra, VentasObjetivo);
            var res = motor.Simular(Visitas, DesdeVisita, HastaVisita);
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

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
