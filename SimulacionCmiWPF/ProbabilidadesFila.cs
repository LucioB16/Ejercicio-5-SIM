using System;
using System.ComponentModel;

namespace SimulacionCmiWPF;

/// <summary>
/// Representa una fila de probabilidades condicionales.
/// </summary>
public class ProbabilidadesFila : INotifyPropertyChanged, IDataErrorInfo
{
    private double _no;
    private double _dudoso;
    private double _si;

    /// <summary>Probabilidad de respuesta "Definitivamente no".</summary>
    public double No
    {
        get => _no;
        set { _no = value; OnPropertyChanged(nameof(No)); }
    }

    /// <summary>Probabilidad de respuesta "Dudoso".</summary>
    public double Dudoso
    {
        get => _dudoso;
        set { _dudoso = value; OnPropertyChanged(nameof(Dudoso)); }
    }

    /// <summary>Probabilidad de respuesta "Definitivamente sí".</summary>
    public double Si
    {
        get => _si;
        set { _si = value; OnPropertyChanged(nameof(Si)); }
    }

    public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            double val = columnName switch
            {
                nameof(No) => No,
                nameof(Dudoso) => Dudoso,
                nameof(Si) => Si,
                _ => 0
            };
            if (val <= 0 || val >= 1)
                return "Debe estar entre 0 y 1";
            if (Math.Abs(No + Dudoso + Si - 1.0) > 0.0001)
                return "La suma debe ser 1,00";
            return string.Empty;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
