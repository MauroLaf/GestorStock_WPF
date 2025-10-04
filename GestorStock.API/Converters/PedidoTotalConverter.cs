// GestorStock.API/Converters/PedidoTotalConverter.cs
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GestorStock.Model.Entities;

namespace GestorStock.API.Converters
{
    public class PedidoTotalConverter : IValueConverter
    {
        // value = Pedido
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Pedido p || p.Repuestos == null) return 0m;
            var total = p.Repuestos.Sum(r => (r?.Cantidad ?? 0) * (r?.Precio ?? 0m));
            return total; // en XAML podrías formatear con StringFormat si quieres
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
