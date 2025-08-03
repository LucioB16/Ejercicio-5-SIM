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
    public MotorCmi(double pRecuerda, double[] condRecuerda, double[] condNoRecuerda, double probCompraDudoso, int ventasObjetivo)
    {
        _pRecuerda = pRecuerda;
        _condRecuerda = condRecuerda;
        _condNoRecuerda = condNoRecuerda;
        _probCompraDudoso = probCompraDudoso;
        _ventasObjetivo = ventasObjetivo;
    }

    /// <summary>
    /// Ejecuta la simulación para una cantidad dada de visitas.
    /// </summary>
    /// <param name="visitas">Visitas a simular.</param>
    /// <param name="desde">Desde qué visita se almacenan vectores.</param>
    /// <param name="hasta">Hasta qué visita se almacenan vectores.</param>
    /// <param name="semilla">Semilla opcional para el generador aleatorio.</param>
    public (List<VectorEstado> Vectores, double ProbabilidadSi, int VentasTotales, int? VisitaObjetivo) Simular(int visitas, int desde, int hasta, int? semilla = null)
    {
        var rng = semilla.HasValue ? new Random(semilla.Value) : new Random();
        var resultados = new List<VectorEstado>();
        int acumSi = 0, acumNo = 0, acumDudoso = 0, ventas = 0;
        int? visitaObjetivo = null;

        for (int i = 1; i <= visitas; i++)
        {
            double r1 = rng.NextDouble();
            bool recuerda = r1 < _pRecuerda;
            double r2 = rng.NextDouble();
            string respuesta;
            double[] dist = recuerda ? _condRecuerda : _condNoRecuerda;
            if (r2 < dist[0])
                respuesta = "Definitivamente no";
            else if (r2 < dist[0] + dist[1])
                respuesta = "Dudoso";
            else
                respuesta = "Definitivamente sí";

            bool compra = false;
            double? r3 = null;
            if (respuesta == "Definitivamente sí")
            {
                compra = true;
            }
            else if (respuesta == "Dudoso")
            {
                r3 = rng.NextDouble();
                compra = r3 < _probCompraDudoso;
            }

            if (respuesta == "Definitivamente sí")
                acumSi++;
            else if (respuesta == "Definitivamente no")
                acumNo++;
            else
                acumDudoso++;

            if (compra)
                ventas++;

            double probAcumSi = (double)acumSi / i;
            var vector = new VectorEstado
            {
                Visita = i,
                RndRecuerda = r1,
                Recuerda = recuerda,
                RndRespuesta = r2,
                Respuesta = respuesta,
                RndCompraDudoso = r3,
                Compra = compra,
                AcumSi = acumSi,
                AcumNo = acumNo,
                AcumDudoso = acumDudoso,
                ProbAcumSi = probAcumSi,
                VentasAcum = ventas
            };

            if (i >= desde && i <= hasta)
                resultados.Add(vector);

            if (visitaObjetivo is null && ventas >= _ventasObjetivo)
                visitaObjetivo = i;
        }

        double probSi = (double)acumSi / visitas;
        return (resultados, probSi, ventas, visitaObjetivo);
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
