using SimulacionCmiCore;
using Xunit;

namespace SimulacionCmiCore.Tests;

public class MotorCmiTests
{
    [Fact]
    public void ProbabilidadDefSiAproximaAnalitico()
    {
        var motor = new MotorCmi(0.35, new[] { 0.55, 0.15, 0.30 }, new[] { 0.70, 0.25, 0.05 }, 0.60, 10000);
        var res = motor.Simular(80000, 1, 1, 123);
        double analitico = 0.35 * 0.30 + 0.65 * 0.05;
        Assert.InRange(res.ProbabilidadSi, analitico - 0.01, analitico + 0.01);
    }

    [Fact]
    public void VisitasParaDiezMilVentasCercanasAnalitico()
    {
        var motor = new MotorCmi(0.35, new[] { 0.55, 0.15, 0.30 }, new[] { 0.70, 0.25, 0.05 }, 0.60, 10000);
        var res = motor.Simular(80000, 1, 1, 123);
        double analitico = motor.CalcularVisitasAnaliticas();
        Assert.True(res.VisitaObjetivo.HasValue);
        Assert.InRange(res.VisitaObjetivo.Value, analitico - 1000, analitico + 1000);
    }

    [Fact]
    public void MotorNoGuardaVectoresFueraDelRango()
    {
        var motor = new MotorCmi(0.35, new[] { 0.55, 0.15, 0.30 }, new[] { 0.70, 0.25, 0.05 }, 0.60, 10000);
        int visitas = 1000;
        int desde = 10;
        int hasta = 20;
        var res = motor.Simular(visitas, desde, hasta, 123);

        int esperado = hasta - desde + 1;
        Assert.Equal(esperado, res.Vectores.Count);
        Assert.Equal(visitas, res.UltimoVector.Visita);
    }
}
