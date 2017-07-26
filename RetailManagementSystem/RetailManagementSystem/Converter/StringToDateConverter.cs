using System;
using System.Globalization;
using System.Windows.Data;

namespace RetailManagementSystem.Converter
{
    public class StringToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }            
            try
            {
                //var indexOfWhiteSpace = value.ToString().Trim().LastIndexOf(" ");
                var dateValue = value.ToString().Split(' ');
                //d = System.Convert.ToDateTime(value, "dd/MM/yyyy");
                var dat = DateTime.ParseExact(dateValue[0], "d/M/yyyy", CultureInfo.InvariantCulture);
                return dat.ToShortDateString();
                //return d;
                // The WPF code that uses this converter will then use the stringformat 
                // attribute in binding to display just HH:mm part of the date as required.
            }
            catch (Exception)
            {
                // If we're unable to convert the value, then best send the value as is to the UI.
                return value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            DateTime result = new DateTime();

            if(DateTime.TryParseExact(value.ToString(), "d/M/yyyy", null,DateTimeStyles.None,out result))
            {
                return result.ToShortDateString();
            }

            return null;                       
        }
    }
}
