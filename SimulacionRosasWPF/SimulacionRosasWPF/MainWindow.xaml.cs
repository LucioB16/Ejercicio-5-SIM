using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SimulacionRosasWPF
{
    /// <summary>
    /// Ventana principal de la simulación Monte Carlo de rosas.
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<EntradaDemanda> DistDemandaSoleado { get; } = new();
        public ObservableCollection<EntradaDemanda> DistDemandaNublado { get; } = new();
        public ObservableCollection<VectorEstado> ResultadosSimulacion { get; } = new();

        private Random _aleatorio = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Carga valores por defecto
            DistDemandaSoleado.Add(new EntradaDemanda(14, 0.05));
            DistDemandaSoleado.Add(new EntradaDemanda(17, 0.20));
            DistDemandaSoleado.Add(new EntradaDemanda(20, 0.45));
            DistDemandaSoleado.Add(new EntradaDemanda(24, 0.30));

            DistDemandaNublado.Add(new EntradaDemanda( 9, 0.05));
            DistDemandaNublado.Add(new EntradaDemanda(11, 0.15));
            DistDemandaNublado.Add(new EntradaDemanda(14, 0.35));
            DistDemandaNublado.Add(new EntradaDemanda(16, 0.25));
            DistDemandaNublado.Add(new EntradaDemanda(20, 0.20));
            
            Politica_SelectionChanged(null, null);
        }
        
        /// <summary>
        /// Muestra/oculta txtPenalizacion o txtCostoExtra según política seleccionada.
        /// </summary>
        private void Politica_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool esFaltante = cmbPolitica.SelectedIndex == 2;

            // Cuando es política 2 mostramos sólo CostoExtra
            lblPenalizacion.Visibility = txtPenalizacion.Visibility = esFaltante
                ? Visibility.Collapsed
                : Visibility.Visible;

            lblCostoExtra.Visibility = txtCostoExtra.Visibility = esFaltante
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        private void AlClicCalcular(object sender, RoutedEventArgs e)
        {
            ResultadosSimulacion.Clear();
            
            // Validar semilla
            string semillaTxt = txtSemilla.Text.Trim();
            if (string.IsNullOrEmpty(semillaTxt))
            {
                _aleatorio = new Random();
            }
            else
            {
                if (!ValidarEntero(semillaTxt, "Semilla RNG", 1, int.MaxValue, out int semilla, out string mensaje))
                {
                    MessageBox.Show(mensaje, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _aleatorio = new Random(semilla);
            }
            
            // 1) Validación de enteros
            if (!ValidarEntero(txtDias.Text, "Días a simular", 1, int.MaxValue, out int dias, out string error) ||
                !ValidarEntero(txtDesde.Text, "Desde día", 1, dias, out int desde, out error) ||
                !ValidarEntero(txtHasta.Text, "Hasta día", desde, dias, out int hasta, out error))
            {
                MessageBox.Show(error, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (!ValidarEntero(txtCompraInicial.Text, "Compra inicial", 1, int.MaxValue, out int compraInicial, out error))
            {
                MessageBox.Show(error, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2) Validación de double
            if (!ValidarDouble(txtPrecio.Text, "Precio por rosa", 0.01, double.MaxValue, out double precioRosa, out error))
            {
                MessageBox.Show(error, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidarDouble(txtCostoDocena.Text, "Costo por docena", 0.01, double.MaxValue, out double costoDocena, out error))
            {
                MessageBox.Show(error, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidarDouble(txtSalvamento.Text, "Ingreso por salvamento", 0.01, double.MaxValue, out double salvamento, out error))
            {
                MessageBox.Show(error, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidarDouble(txtPenalizacion.Text, "Penalización rosa faltante", 0.01, double.MaxValue, out double penalizacion, out error))
            {
                MessageBox.Show(error, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidarDouble(txtCostoExtra.Text, "Costo extra por docena faltante", 0.01, double.MaxValue, out double costoExtra, out error))
            {
                MessageBox.Show(error, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3) Probabilidades
            if (!ValidarDouble(txtProbSoleado.Text, "P(soleado)", 0.01, 0.99, out double pSoleado, out error))
            {
                MessageBox.Show(error, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidarDouble(txtProbNublado.Text, "P(nublado/lluvia)", 0.01, 0.99, out double pNublado, out error))
            {
                MessageBox.Show(error, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Math.Abs(pSoleado + pNublado - 1.0) > 0.001)
            {
                MessageBox.Show("La suma de P(soleado) y P(nublado/lluvia) debe ser igual a 1.", "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // 4) Validar tablas de distribución
            TruncarDistribucion(DistDemandaSoleado);
            TruncarDistribucion(DistDemandaNublado);
            if (!ValidarDistribucion(DistDemandaSoleado, "Soleado")) return;
            if (!ValidarDistribucion(DistDemandaNublado, "Nublado/lluvia")) return;
            
            // Distribución conjunta
            var climaDist = new[]
            {
                (DistDemandaSoleado.AsEnumerable(), pSoleado),
                (DistDemandaNublado.AsEnumerable(), pNublado)
            };

            double ganAcumulada = 0;
            double ganMediaAnterior = 0;

            // Inicial stock anterior igual a parámetro de docenas a comprar
            int stockAnterior = compraInicial;

            for (int dia = 1; dia <= dias; dia++)
            {
                // 1. Generar clima
                double rndC = _aleatorio.NextDouble();
                bool esSoleado = rndC <= pSoleado;
                var distSeleccionada = esSoleado
                    ? DistDemandaSoleado
                    : DistDemandaNublado;
                string clima = esSoleado ? "Soleado" : "Nublado";

                // 2. Generar demanda
                double rndD = _aleatorio.NextDouble();
                int demanda = SeleccionarDemanda(distSeleccionada, rndD);

                // 3. Definir cantidad a comprar según política
                int compra;
                int indicePolitica = cmbPolitica.SelectedIndex;
                switch (indicePolitica)
                {
                    case 0: // compra fija
                        compra = stockAnterior;
                        break;
                    case 1: // demanda día anterior
                        compra = stockAnterior;
                        break;
                    default: // demanda anterior + compra faltante
                        compra = stockAnterior;
                        break;
                }

                // 4. Calcular ventas, faltantes, sobrantes
                int venta = indicePolitica == 2 ? demanda : Math.Min(compra, demanda); // Si es politica de compra faltante entonces siempre venta = demanda
                int faltante = Math.Max(demanda - compra, 0);
                int sobrante = Math.Max(compra - demanda, 0);

                // 5. Costos e ingresos
                double ingresoVentas = venta * 12 * precioRosa;
                double costoCompra = compra * costoDocena;
                double ingresoSobrante = sobrante * 12 * salvamento;
                double costoFaltante = (indicePolitica == 2)
                    ? faltante * costoExtra
                    : faltante * 12 * penalizacion;

                double gananciaDiaria = ingresoVentas
                                       + ingresoSobrante
                                       - costoCompra
                                       - costoFaltante;
                ganAcumulada += gananciaDiaria;
                double ganMedia = ((dia - 1) * ganMediaAnterior + gananciaDiaria) / dia;

                if (dia >= desde && dia <= hasta)
                {
                    // 6. Registrar resultados fila
                    ResultadosSimulacion.Add(new VectorEstado
                    {
                        Dia = dia,
                        RandClima = rndC,
                        Clima = clima,
                        RandDemanda = rndD,
                        CantDemanda = demanda,
                        CantCompra = compra,
                        CantVenta = venta,
                        CantFaltante = faltante,
                        CantSobrante = sobrante,
                        GanVentas = ingresoVentas,
                        CostoCompra = costoCompra,
                        GanSobrante = ingresoSobrante,
                        CostoFaltante = costoFaltante,
                        GananciaDiaria = gananciaDiaria,
                        GananciaAcumulada = ganAcumulada,
                        GananciaMedia = ganMedia
                    });
                }
                
                // 7. Actualizar stockAnterior para siguiente día según demanda
                stockAnterior = (indicePolitica == 0)
                    ? stockAnterior
                    : demanda;
                
                // 8. Actualizar ganancia media anterior para siguiente ganancia media
                ganMediaAnterior = ganMedia;
            }

            double mediaFinal = ganMediaAnterior;
            
            var ventanaRes = new VentanaResultados(ResultadosSimulacion, mediaFinal, dias) { Owner = this };
            ventanaRes.ShowDialog();
        }

        private int SeleccionarDemanda(
            IEnumerable<EntradaDemanda> dist,
            double rnd)
        {
            double cumul = 0;
            foreach (var e in dist)
            {
                cumul += e.Probabilidad;
                if (rnd <= cumul)
                    return e.Docenas;
            }
            return dist.Last().Docenas;
        }
        
        private bool ValidarEntero(string texto, string nombre, int min, int max, out int valor, out string mensaje)
        {
            mensaje = string.Empty;
            if (!int.TryParse(texto, out valor))
            {
                mensaje = $"{nombre} debe ser un entero válido.";
                return false;
            }
            if (valor < min || valor > max)
            {
                mensaje = $"{nombre} debe estar entre {min} y {max}.";
                return false;
            }
            return true;
        }

        private bool ValidarDouble(string texto, string nombre, double min, double max, out double valor, out string mensaje)
        {
            mensaje = string.Empty;
            var cultura = CultureInfo.CurrentCulture;
            if (!double.TryParse(texto, NumberStyles.Number, cultura, out valor))
            {
                mensaje = $"{nombre} debe ser un número decimal válido.";
                return false;
            }
            // Hasta dos decimales
            var redondeado = Math.Round(valor, 2);
            if (Math.Abs(valor - redondeado) > 0.000001)
            {
                mensaje = $"{nombre} solo puede tener hasta dos decimales.";
                return false;
            }
            if (valor < min || valor > max)
            {
                mensaje = $"{nombre} debe estar entre {min:0.##} y {max:0.##}.";
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Valida que en <paramref name="dist"/>:
        /// - Docenas enteras [0, int.MaxValue], sin duplicados.
        /// - Probabilidades con 2 decimales, entre 0.01 y 0.99.
        /// - Suma de probabilidades = 1.
        /// </summary>
        private bool ValidarDistribucion(
            ObservableCollection<EntradaDemanda> dist,
            string nombre)
        {
            // 1. Docenas válidas y sin duplicados
            var docs = dist.Select(d => d.Docenas).ToList();
            if (docs.Any(d => d < 0))
            {
                MessageBox.Show(
                    $"En la distribución \"{nombre}\": todas las Docenas deben ser ≥ 0.",
                    "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (docs.Count != docs.Distinct().Count())
            {
                MessageBox.Show(
                    $"En la distribución \"{nombre}\": no puede haber dos filas con la misma cantidad de Docenas.",
                    "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // 2. Probabilidades válidas
            foreach (var entrada in dist)
            {
                int docenas = entrada.Docenas;
                double p = entrada.Probabilidad;
                // chequeo de rango
                if (p < 0.01 || p > 0.99)
                {
                    MessageBox.Show(
                        $"En \"{nombre}\": la probabilidad para {docenas} docenas debe estar entre 0,01 y 0,99.",
                        "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            // 3. Suma = 1
            double suma = dist.Sum(d => d.Probabilidad);
            if (Math.Abs(suma - 1.0) > 0.001)
            {
                MessageBox.Show(
                    $"La suma de probabilidades en \"{nombre}\" debe ser = 1 (actualmente {suma:F2}).",
                    "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Reemplaza cada entrada en la colección por una nueva donde Probabilidad
        /// ha sido truncada a dos decimales (p.ej. 0,456 → 0,45).
        /// </summary>
        private void TruncarDistribucion(ObservableCollection<EntradaDemanda> dist)
        {
            for (int i = 0; i < dist.Count; i++)
            {
                var entrada = dist[i];
                double p = entrada.Probabilidad;
                double truncado = Math.Truncate(p * 100) / 100.0;
                // Sobrescribimos el registro con la versión truncada
                dist[i] = new EntradaDemanda(entrada.Docenas, truncado);
            }
        }
    }

    public record EntradaDemanda(int Docenas, double Probabilidad);
}
