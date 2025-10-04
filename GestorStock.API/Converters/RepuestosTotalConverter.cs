// GestorStock.API/Converters/RepuestosTotalConverter.cs
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GestorStock.Model.Entities;

namespace GestorStock.API.Converters
{
    public class RepuestosTotalConverter : IValueConverter
    {
        // value = Pedido
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Pedido p || p.Repuestos == null) return 0m;
            return p.Repuestos.Sum(r => (r?.Cantidad ?? 0) * (r?.Precio ?? 0m));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
