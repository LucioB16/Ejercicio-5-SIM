using System.Globalization;
using System.Windows;
using ScottPlot;

namespace SimulacionRosasWPF
{
    /// <summary>
    /// Ventana que muestra el gráfico de evolución de la ganancia media.
    /// </summary>
    public partial class VentanaGrafico : Window
    {
        private readonly List<(int Dia, double GananciaMedia)> _puntos;

        public VentanaGrafico(IEnumerable<(int Dia, double GananciaMedia)> puntos)
        {
            InitializeComponent();
            _puntos = puntos.ToList();
            Loaded += VentanaGrafico_Loaded;
        }

        private void VentanaGrafico_Loaded(object sender, RoutedEventArgs e)
        {
            // 1) Extraer datos
            double[] xs = _puntos.Select(p => (double)p.Dia).ToArray();
            double[] ys = _puntos.Select(p => p.GananciaMedia).ToArray();

            // 2) Agregar una serie de línea (sin marcadores)
            var serie = WpfPlot1.Plot.Add.Scatter(xs, ys);  // <<< aquí uso Add.Scatter(...)
            serie.MarkerSize = 0;
            serie.LineWidth  = 2;

            // 3) Etiquetas
            WpfPlot1.Plot.Title("Evolución de Ganancia Media por Día");
            WpfPlot1.Plot.XLabel("Día");
            WpfPlot1.Plot.YLabel("Ganancia Media ($ Pesos)");

            // 4) Escalar ejes
            WpfPlot1.Plot.Axes.AutoScale();
            
            // ——— Añadimos texto en el último punto ———
            // Tomamos la tupla (día, ganancia) del último elemento
            var ultimo = _puntos.Last();
            // creo cultura Argentina
            var culturaARS = CultureInfo.CreateSpecificCulture("es-AR");
            // formateo la ganancia como moneda ARS
            string texto = ultimo.GananciaMedia.ToString("C", culturaARS);
            // Creamos la etiqueta en ese punto
            var etiqueta = WpfPlot1.Plot.Add.Text(
                text: texto, 
                x: ultimo.Dia, 
                y: ultimo.GananciaMedia);
            // Estilo de la etiqueta
            etiqueta.LabelFontSize       = 14;
            etiqueta.LabelBorderColor    = Colors.Black;
            etiqueta.LabelBorderWidth    = 1;
            etiqueta.LabelPadding        = 4;
            etiqueta.LabelBold           = true;
            etiqueta.LabelBackgroundColor= Colors.White;
            // Ajustamos el offset en pixeles para que no tape el punto
            etiqueta.OffsetX = 10;
            etiqueta.OffsetY = -10;

            // 5) Refrescar
            WpfPlot1.Refresh();
        }
    }
}