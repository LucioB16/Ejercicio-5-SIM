namespace SimulacionRosasWPF;

public class VectorEstado
{
    public int Dia { get; set; }
    public double RandClima { get; set; }
    public string Clima { get; set; }
    public double RandDemanda { get; set; }
    public int CantDemanda { get; set; }
    public int CantCompra { get; set; }
    public int CantVenta { get; set; }
    public int CantFaltante { get; set; }
    public int CantSobrante { get; set; }
    public double GanVentas { get; set; }
    public double CostoCompra { get; set; }
    public double GanSobrante { get; set; }
    public double CostoFaltante { get; set; }
    public double GananciaDiaria { get; set; }
    public double GananciaAcumulada { get; set; }
    public double GananciaMedia { get; set; }
}