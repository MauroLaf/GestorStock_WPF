using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Collections.Generic;
using GestorStock.Model.Entities;

namespace GestorStock.API.Converters
{
    public class RepuestosTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection<Repuesto> repuestos)
            {
                return repuestos.Sum(r => r.Cantidad * r.Precio);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}