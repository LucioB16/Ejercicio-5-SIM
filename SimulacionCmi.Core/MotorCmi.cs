using System;
using System.Collections.Generic;

namespace SimulacionCmi.Core;

/// <summary>
/// Motor de simulación para el problema de CMI Corporation.
/// </summary>
public class MotorCmi
{
    private readonly Random _aleatorio;

    /// <summary>
    /// Crea una nueva instancia del motor.
    /// </summary>
    /// <param name="semilla">Semilla opcional para el generador pseudoaleatorio.</param>
    public MotorCmi(int? semilla = null)
    {
        _aleatorio = semilla.HasValue ? new Random(semilla.Value) : new Random();
    }

    /// <summary>
    /// Ejecuta la simulación de visitas.
    /// </summary>
    /// <param name="cantidadVisitas">Cantidad total de visitas a simular.</param>
    /// <param name="probRecuerda">Probabilidad de que el individuo recuerde el anuncio.</param>
    /// <param name="respSiRecuerda">Distribución de respuestas cuando recuerda.</param>
    /// <param name="respNoRecuerda">Distribución de respuestas cuando no recuerda.</param>
    /// <param name="probCompraDudoso">Probabilidad de que un dudoso termine comprando.</param>
    /// <param name="ventaObjetivo">Ventas objetivo para calcular la visita en que se alcanza.</param>
    /// <param name="desde">Primer índice de visita a conservar.</param>
    /// <param name="hasta">Último índice de visita a conservar.</param>
    /// <returns>Resultado de la simulación.</returns>
    public ResultadoSimulacion Simular(
        int cantidadVisitas,
        double probRecuerda,
        DistribucionRespuesta respSiRecuerda,
        DistribucionRespuesta respNoRecuerda,
        double probCompraDudoso,
        int ventaObjetivo,
        int desde,
        int hasta)
    {
        var vectores = new List<VectorEstado>();

        int acumSi = 0;
        int acumNo = 0;
        int acumDudoso = 0;
        int ventasAcum = 0;
        int? visitaObjetivo = null;

        VectorEstado ultimoVector = null!;

        for (int i = 1; i <= cantidadVisitas; i++)
        {
            double randRecuerda = _aleatorio.NextDouble();
            bool recuerda = randRecuerda < probRecuerda;

            double randResp = _aleatorio.NextDouble();
            RespuestaCliente respuesta;
            if (recuerda)
            {
                respuesta = ObtenerRespuesta(randResp, respSiRecuerda);
            }
            else
            {
                respuesta = ObtenerRespuesta(randResp, respNoRecuerda);
            }

            double? randCompra = null;
            bool compra = false;
            switch (respuesta)
            {
                case RespuestaCliente.DefinitivamenteSi:
                    compra = true;
                    acumSi++;
                    break;
                case RespuestaCliente.DefinitivamenteNo:
                    acumNo++;
                    break;
                case RespuestaCliente.Dudoso:
                    acumDudoso++;
                    randCompra = _aleatorio.NextDouble();
                    compra = randCompra < probCompraDudoso;
                    break;
            }

            if (compra)
            {
                ventasAcum++;
                if (visitaObjetivo == null && ventasAcum >= ventaObjetivo)
                {
                    visitaObjetivo = i;
                }
            }

            double probAcumSi = (double)acumSi / i;
            double probAcumNo = (double)acumNo / i;
            double probAcumDudoso = (double)acumDudoso / i;

            var vector = new VectorEstado
            {
                Visita = i,
                RandRecuerda = randRecuerda,
                RecuerdaMensaje = recuerda,
                RandRespuesta = randResp,
                Respuesta = respuesta,
                RandCompra = randCompra,
                Compra = compra,
                AcumSi = acumSi,
                AcumNo = acumNo,
                AcumDudoso = acumDudoso,
                ProbAcumSi = probAcumSi,
                ProbAcumNo = probAcumNo,
                ProbAcumDudoso = probAcumDudoso,
                VentasAcumuladas = ventasAcum
            };

            if (i >= desde && i <= hasta)
            {
                vectores.Add(vector);
            }

            ultimoVector = vector;
        }

        return new ResultadoSimulacion(vectores, visitaObjetivo, ultimoVector);
    }

    private static RespuestaCliente ObtenerRespuesta(double rnd, DistribucionRespuesta dist)
    {
        if (rnd < dist.DefinitivamenteNo)
            return RespuestaCliente.DefinitivamenteNo;
        if (rnd < dist.DefinitivamenteNo + dist.Dudoso)
            return RespuestaCliente.Dudoso;
        return RespuestaCliente.DefinitivamenteSi;
    }

    /// <summary>
    /// Calcula la cantidad de visitas necesarias mediante la fórmula analítica.
    /// </summary>
    /// <param name="probRecuerda">Probabilidad de recordar el anuncio.</param>
    /// <param name="respSiRecuerda">Distribución de respuestas cuando recuerda.</param>
    /// <param name="respNoRecuerda">Distribución de respuestas cuando no recuerda.</param>
    /// <param name="probCompraDudoso">Probabilidad de compra de un dudoso.</param>
    /// <param name="ventasObjetivo">Cantidad de ventas objetivo.</param>
    /// <returns>Cantidad teórica de visitas necesarias.</returns>
    public static double CalcularVisitasAnaliticas(
        double probRecuerda,
        DistribucionRespuesta respSiRecuerda,
        DistribucionRespuesta respNoRecuerda,
        double probCompraDudoso,
        int ventasObjetivo)
    {
        double pDefSi = probRecuerda * respSiRecuerda.DefinitivamenteSi +
                         (1 - probRecuerda) * respNoRecuerda.DefinitivamenteSi;
        double pDudoso = probRecuerda * respSiRecuerda.Dudoso +
                         (1 - probRecuerda) * respNoRecuerda.Dudoso;
        double pVenta = pDefSi + probCompraDudoso * pDudoso;
        return ventasObjetivo / pVenta;
    }
}

/// <summary>
/// Resultado completo de la simulación.
/// </summary>
public record ResultadoSimulacion(
    IReadOnlyList<VectorEstado> Vectores,
    int? VisitaObjetivo,
    VectorEstado UltimoVector);

/// <summary>
/// Distribución de respuestas para un escenario.
/// </summary>
public record struct DistribucionRespuesta(
    double DefinitivamenteNo,
    double Dudoso,
    double DefinitivamenteSi);
