using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPF_ManageCourt.Utils
{
    public class BoolToIsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string paramString && bool.TryParse(paramString, out var targetBool))
            {
                return boolValue == targetBool;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && parameter is string paramString && bool.TryParse(paramString, out var targetBool))
            {
                return isChecked == true ? targetBool : !targetBool;
            }
            return false;
        }
    }
}
