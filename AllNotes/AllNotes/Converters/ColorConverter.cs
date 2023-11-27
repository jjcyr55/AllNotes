using System;
using System.Globalization;
using Xamarin.Forms;
using AllNotes.Enum;
using Xamarin.CommunityToolkit.Converters;

namespace AllNotes.Converters
{
    public class ColorConverter : IValueConverter
    {
        public static Color Yellow = Color.FromHex("#FCEE96");
        public static Color Green = Color.FromHex("#D2FBA4");
        public static Color Blue = Color.FromHex("#7EB6D7");
        public static Color Purple = Color.FromHex("#D3B5E5");
        public static Color Pink = Color.FromHex("#FFD4DB");
        public static Color Red = Color.FromHex("#E77480");
        public static Color Orange = Color.FromHex("#F7BA8E");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? color = value as int?;
            if (color != null)
            {
                if (color == (int)Colors.Yellow)
                    return Yellow;
                if (color == (int)Colors.Green)
                    return Green;
                if (color == (int)Colors.Blue)
                    return Blue;
                if (color == (int)Colors.Purple)
                    return Purple;
                if (color == (int)Colors.Pink)
                    return Pink;
                if (color == (int)Colors.Red)
                    return Red;
                if (color == (int)Colors.Orange)
                    return Orange;
              
            }

            return Color.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}