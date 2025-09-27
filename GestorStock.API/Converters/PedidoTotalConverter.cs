using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Collections.Generic;
using GestorStock.Model.Entities;

namespace GestorStock.API.Converters
{
    public class PedidoTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection<Item> items)
            {
                // Suma los totales de todos los ítems en el pedido.
                return items.Sum(item => item.Repuestos?.Sum(repuesto => repuesto.Precio * repuesto.Cantidad) ?? 0);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}