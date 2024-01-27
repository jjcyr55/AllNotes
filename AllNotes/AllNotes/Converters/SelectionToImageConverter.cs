using System;
using System.Globalization;
using Xamarin.Forms;

public class SelectionToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isSelected)
        {
            return isSelected ? "check.png" : "empty_circle.png"; // Replace with your actual image names
        }

        return null; // Or a default image
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}