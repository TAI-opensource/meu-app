using System.Globalization;
using System.Windows.Data;

namespace CRM_Alunos.Converters
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
                return isActive ? "Ativo" : "Inativo";
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
                return status == "Ativo";
            return false;
        }
    }
}
