using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AllNotes.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
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

        /*public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Assuming that both value and parameter are of type 'bool'
            bool isSelected = (bool)value;
               bool isEditMode = (bool)parameter;

            // The view is visible only when both isSelected and isEditMode are true
            return isSelected && isEditMode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }*/
    }
}
