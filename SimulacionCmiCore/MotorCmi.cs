using System;
using System.Collections.Generic;

namespace SimulacionCmiCore;

/// <summary>
/// Motor que ejecuta la simulación para el caso de la empresa CMI.
/// </summary>
public class MotorCmi
{
    private readonly double _pRecuerda;
    private readonly double[] _condRecuerda;
    private readonly double[] _condNoRecuerda;
    private readonly double _probCompraDudoso;
    private readonly int _ventasObjetivo;

    /// <summary>
    /// Crea una instancia del motor.
    /// </summary>
    public MotorCmi(double pRecuerda, double[] condRecuerda, double[] condNoRecuerda,
        double probCompraDudoso, int ventasObjetivo)
    {
        _pRecuerda = pRecuerda;
        _condRecuerda = condRecuerda;
        _condNoRecuerda = condNoRecuerda;
        _probCompraDudoso = probCompraDudoso;
        _ventasObjetivo = ventasObjetivo;
    }

    /// <summary>
    /// Ejecuta la simulación.
    /// </summary>
    /// <param name="visitas">Cantidad total de visitas a simular.</param>
    /// <param name="desde">Desde qué visita se copian vectores para la tabla.</param>
    /// <param name="hasta">Hasta qué visita se copian vectores para la tabla.</param>
    /// <param name="semilla">Semilla opcional del generador aleatorio.</param>
    public (List<VectorEstado> Vectores, VectorEstado UltimoVector, double ProbabilidadSi,
        int VentasTotales, int? VisitaObjetivo) Simular(int visitas, int desde, int hasta, int? semilla = null)
    {
        var rng = semilla.HasValue ? new Random(semilla.Value) : new Random();
        var resultados = new List<VectorEstado>();
        VectorEstado? anterior = null;
        VectorEstado? actual = null;

        for (int i = 1; i <= visitas; i++)
        {
            // Aleatorios
            double rndRecuerda = rng.NextDouble();
            bool recuerda = rndRecuerda < _pRecuerda;
            double rndRespuesta = rng.NextDouble();
            string respuesta;
            double[] dist = recuerda ? _condRecuerda : _condNoRecuerda;
            if (rndRespuesta < dist[0])
                respuesta = "Definitivamente no";
            else if (rndRespuesta < dist[0] + dist[1])
                respuesta = "Dudoso";
            else
                respuesta = "Definitivamente sí";

            bool compra = false;
            double? rndCompra = null;
            if (respuesta == "Definitivamente sí")
            {
                compra = true;
            }
            else if (respuesta == "Dudoso")
            {
                rndCompra = rng.NextDouble();
                compra = rndCompra >= 1 - _probCompraDudoso;
            }

            // Acumuladores utilizando el vector anterior
            int acumSi = (anterior?.AcumSi ?? 0) + (respuesta == "Definitivamente sí" ? 1 : 0);
            int acumNo = (anterior?.AcumNo ?? 0) + (respuesta == "Definitivamente no" ? 1 : 0);
            int acumDudoso = (anterior?.AcumDudoso ?? 0) + (respuesta == "Dudoso" ? 1 : 0);
            int ventas = (anterior?.VentasAcum ?? 0) + (compra ? 1 : 0);

            int? indiceMeta = anterior?.IndiceMetaVentas;
            if (indiceMeta is null && ventas >= _ventasObjetivo)
                indiceMeta = i;

            actual = new VectorEstado
            {
                Visita = i,
                RndRecuerda = rndRecuerda,
                Recuerda = recuerda,
                RndRespuesta = rndRespuesta,
                Respuesta = respuesta,
                RndCompra = rndCompra,
                Compra = compra,
                AcumSi = acumSi,
                AcumNo = acumNo,
                AcumDudoso = acumDudoso,
                ProbAcumSi = (double)acumSi / i,
                VentasAcum = ventas,
                IndiceMetaVentas = indiceMeta
            };

            if (i >= desde && i <= hasta)
                resultados.Add(actual.Clonar());

            anterior = actual;
        }

        // al terminar, 'actual' contiene el último vector
        VectorEstado ultimo = actual!;
        double probSi = (double)ultimo.AcumSi / ultimo.Visita;
        int ventasTotales = ultimo.VentasAcum;
        return (resultados, ultimo, probSi, ventasTotales, ultimo.IndiceMetaVentas);
    }

    /// <summary>
    /// Calcula el número esperado de visitas para alcanzar el objetivo mediante el resultado analítico.
    /// </summary>
    public double CalcularVisitasAnaliticas()
    {
        double pSi = _pRecuerda * _condRecuerda[2] + (1 - _pRecuerda) * _condNoRecuerda[2];
        double pDudoso = _pRecuerda * _condRecuerda[1] + (1 - _pRecuerda) * _condNoRecuerda[1];
        double pVenta = pSi + _probCompraDudoso * pDudoso;
        return _ventasObjetivo / pVenta;
    }
}
