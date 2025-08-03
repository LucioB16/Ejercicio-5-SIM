using SimulacionCmi.Core;

namespace SimulacionCmi.Core.Tests;

public class MotorCmiTests
{
    [Fact]
    public void CalcularVisitasAnaliticas_CoincideConFormula()
    {
        var distRecuerda = new DistribucionRespuesta(0.55, 0.15, 0.30);
        var distNoRecuerda = new DistribucionRespuesta(0.70, 0.25, 0.05);

        double visitas = MotorCmi.CalcularVisitasAnaliticas(0.35, distRecuerda, distNoRecuerda, 0.60, 10000);
        Assert.Equal(37523.4521575985, visitas, 10); // precisión de 1e-10
    }

    [Fact]
    public void Simular_GeneraResultadosDeterministicos()
    {
        var motor = new MotorCmi(123);
        var distRecuerda = new DistribucionRespuesta(0.55, 0.15, 0.30);
        var distNoRecuerda = new DistribucionRespuesta(0.70, 0.25, 0.05);

        var resultado = motor.Simular(100, 0.35, distRecuerda, distNoRecuerda, 0.60, 20, 1, 100);

        Assert.Equal(0.12, resultado.UltimoVector.ProbAcumSi, 2);
        Assert.Equal(21, resultado.UltimoVector.VentasAcumuladas);
        Assert.Equal(94, resultado.VisitaObjetivo);
    }
}
