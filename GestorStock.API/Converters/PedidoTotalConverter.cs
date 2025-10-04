using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GestorStock.Model.Entities;

namespace GestorStock.API.Converters
{
    public sealed class PedidoTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Pedido p || p.Repuestos is null) return "0,00";

            decimal total = p.Repuestos.Sum(r =>
                (r?.Precio ?? 0m) * (decimal)(r?.Cantidad ?? 0));

            return total.ToString("0.00", culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
