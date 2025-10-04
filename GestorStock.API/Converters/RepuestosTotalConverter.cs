using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GestorStock.Model.Entities;

namespace GestorStock.API.Converters
{
    public class RepuestosTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Pedido p && p.Repuestos != null)
                return p.Repuestos.Count;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
