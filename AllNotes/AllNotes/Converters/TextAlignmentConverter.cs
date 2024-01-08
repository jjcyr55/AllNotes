using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AllNotes.Converters
{
    public class TextAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var alignment = (string)value;
            return alignment switch
            {
                "Left" => TextAlignment.Start,
                "Center" => TextAlignment.Center,
                "Right" => TextAlignment.End,
                _ => TextAlignment.Start
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
