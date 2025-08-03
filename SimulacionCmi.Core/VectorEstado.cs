namespace SimulacionCmi.Core;

/// <summary>
/// Representa el estado de una visita simulada.
/// </summary>
public class VectorEstado
{
    /// <summary>Índice (1..N) de la visita.</summary>
    public int Visita { get; set; }

    /// <summary>Valor aleatorio utilizado para decidir si recuerda el anuncio.</summary>
    public double RandRecuerda { get; set; }

    /// <summary>Indica si el individuo recordó el anuncio.</summary>
    public bool RecuerdaMensaje { get; set; }

    /// <summary>Valor aleatorio utilizado para decidir la respuesta.</summary>
    public double RandRespuesta { get; set; }

    /// <summary>Respuesta dada por el individuo.</summary>
    public RespuestaCliente Respuesta { get; set; }

    /// <summary>Valor aleatorio utilizado para decidir la compra de un dudoso.</summary>
    public double? RandCompra { get; set; }

    /// <summary>Indica si la visita terminó en compra.</summary>
    public bool Compra { get; set; }

    /// <summary>Cantidad acumulada de respuestas "definitivamente sí".</summary>
    public int AcumSi { get; set; }

    /// <summary>Cantidad acumulada de respuestas "definitivamente no".</summary>
    public int AcumNo { get; set; }

    /// <summary>Cantidad acumulada de respuestas "dudoso".</summary>
    public int AcumDudoso { get; set; }

    /// <summary>Probabilidad acumulada de respuestas "definitivamente sí".</summary>
    public double ProbAcumSi { get; set; }

    /// <summary>Probabilidad acumulada de respuestas "definitivamente no".</summary>
    public double ProbAcumNo { get; set; }

    /// <summary>Probabilidad acumulada de respuestas "dudoso".</summary>
    public double ProbAcumDudoso { get; set; }

    /// <summary>Ventas acumuladas hasta esta visita.</summary>
    public int VentasAcumuladas { get; set; }
}

/// <summary>
/// Posibles respuestas del cliente.
/// </summary>
public enum RespuestaCliente
{
    /// <summary>Respuesta negativa.</summary>
    DefinitivamenteNo,

    /// <summary>Respuesta dudosa.</summary>
    Dudoso,

    /// <summary>Respuesta positiva.</summary>
    DefinitivamenteSi
}
