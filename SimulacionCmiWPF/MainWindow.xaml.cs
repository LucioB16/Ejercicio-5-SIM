using System;
using System.Windows;
using SimulacionCmiCore;

namespace SimulacionCmiWPF;

/// <summary>
/// Ventana principal minimal para ejecutar la simulación.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Simular_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(txtVisitas.Text, out int visitas)) visitas = 40000;
        int? semilla = null;
        if (int.TryParse(txtSemilla.Text, out int s)) semilla = s;

        var motor = new MotorCmi(0.35,
            [0.55, 0.15, 0.30],
            [0.70, 0.25, 0.05],
            0.60, 10000);
        var res = motor.Simular(visitas, 1, visitas, semilla);
        lblResultado.Text = $"P(Def. Sí) ≈ {res.ProbabilidadSi:F4} | Ventas: {res.VentasTotales}";
    }
}
