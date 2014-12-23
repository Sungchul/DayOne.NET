/*
    Jarloo
    http://www.jarloo.com
 
    This work is licensed under a Creative Commons Attribution-ShareAlike 3.0 Unported License  
    http://creativecommons.org/licenses/by-sa/3.0/ 

*/
using System;
using System.Globalization;
using System.Windows.Data;

namespace DayOne.NET.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime) value;

            string param = (string) parameter;

            switch(param.ToUpper())
            {
                case "MONTH":
                    return date.Month;
                case "YEAR":
                    return date.Year;
                case "DAY":
                    return date.Day;
                default:
                    return date.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}