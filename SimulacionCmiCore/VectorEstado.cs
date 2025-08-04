namespace SimulacionCmiCore;

/// <summary>
/// Representa una visita simulada para el problema de CMI.
/// </summary>
public class VectorEstado
{
    /// <summary>Índice de la visita.</summary>
    public int Visita { get; set; }
    /// <summary>Número aleatorio para decidir si recuerda el anuncio.</summary>
    public double RndRecuerda { get; set; }
    /// <summary>Indica si el individuo recuerda el mensaje.</summary>
    public bool Recuerda { get; set; }
    /// <summary>Número aleatorio para determinar la respuesta.</summary>
    public double RndRespuesta { get; set; }
    /// <summary>Respuesta dada por el individuo.</summary>
    public string Respuesta { get; set; } = string.Empty;
    /// <summary>Número aleatorio usado para decidir la compra si la respuesta fue "dudoso".</summary>
    public double? RndCompra { get; set; }
    /// <summary>Indica si la visita termina en compra.</summary>
    public bool Compra { get; set; }
    /// <summary>Cantidad acumulada de respuestas "definitivamente sí".</summary>
    public int AcumSi { get; set; }
    /// <summary>Cantidad acumulada de respuestas "definitivamente no".</summary>
    public int AcumNo { get; set; }
    /// <summary>Cantidad acumulada de respuestas "dudoso".</summary>
    public int AcumDudoso { get; set; }
    /// <summary>Probabilidad acumulada de respuestas "definitivamente sí".</summary>
    public double ProbAcumSi { get; set; }
    /// <summary>Ventas acumuladas.</summary>
    public int VentasAcum { get; set; }

    /// <summary>
    /// Índice de la visita en la que se alcanzó por primera vez la meta de ventas.
    /// </summary>
    public int? IndiceMetaVentas { get; init; }

    /// <summary>
    /// Crea una copia superficial del vector.
    /// </summary>
    public VectorEstado Clonar() => (VectorEstado)MemberwiseClone();
}
