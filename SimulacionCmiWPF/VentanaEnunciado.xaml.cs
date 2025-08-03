using System.IO;
using System.Windows;

namespace SimulacionCmiWPF;

/// <summary>
/// Ventana que muestra el enunciado del problema.
/// </summary>
public partial class VentanaEnunciado : Window
{
    public VentanaEnunciado()
    {
        InitializeComponent();
        CargarEnunciado();
    }

    private void CargarEnunciado()
    {
        string ruta = Path.Combine(AppContext.BaseDirectory, "README.md");
        if (File.Exists(ruta))
            txtEnunciado.Text = File.ReadAllText(ruta);
        else
            txtEnunciado.Text = "README no encontrado.";
    }
}
