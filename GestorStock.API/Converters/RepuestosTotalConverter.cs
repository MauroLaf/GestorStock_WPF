using System.Globalization;
using System.Windows.Data;

public class RepuestosTotalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is System.Collections.Generic.ICollection<Repuesto> repuestos)
            return repuestos.Sum(r => r.Cantidad * r.Precio);
        return 0m;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
