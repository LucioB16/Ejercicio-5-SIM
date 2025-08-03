using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace SimulacionRosasWPF;

/// <summary>
/// Ventana emergente que muestra los resultados de la simulación.
/// </summary>
public partial class VentanaResultados : Window
{
    /// <summary>
    /// Colección a visualizar en el DataGrid.
    /// </summary>
    public ObservableCollection<VectorEstado> Resultados { get; }

    public VentanaResultados(
        ObservableCollection<VectorEstado> resultados,
        double gananciaMediaFinal,
        int ultimoDia)
    {
        InitializeComponent();
        Resultados = resultados;
        DataContext = this;
        
        // Mostrar la ganancia media del último día en la etiqueta
        lblMediaFinal.Text =
            $"Ganancia media día {ultimoDia}: {gananciaMediaFinal.ToString("C", 
                CultureInfo.CreateSpecificCulture("es-AR"))}";
    }

    /// <summary>
    /// Al hacer clic en "Ver gráfico", extrae los pares (Día, GananciaMedia)
    /// y muestra un mensaje (aquí puedes invocar tu librería de gráficas).
    /// </summary>
    private void AlClicVerGrafico(object sender, RoutedEventArgs e)
    {
        var puntos = Resultados
            .Select(r => (r.Dia, r.GananciaMedia))
            .ToList();

        var ventanaGraf = new VentanaGrafico(puntos) { Owner = this };
        
        ventanaGraf.ShowDialog();
    }
}