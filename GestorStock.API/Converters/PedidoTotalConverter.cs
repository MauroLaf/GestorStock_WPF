using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GestorStock.Model.Entities;

namespace GestorStock.API.Converters
{
    public class PedidoTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Pedido p && p.Repuestos != null)
                return p.Repuestos.Sum(r => r.Cantidad * r.Precio);
            return 0m;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
