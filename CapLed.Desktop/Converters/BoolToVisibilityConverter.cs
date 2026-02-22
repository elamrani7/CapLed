using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CapLed.Desktop.Converters;

/// <summary>
/// Converts bool → Visibility.
/// true  → Visible
/// false → Collapsed  (or Hidden if parameter is "Hidden")
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = value is bool b && b;
        Visibility falseValue = (parameter as string) == "Hidden"
            ? Visibility.Hidden
            : Visibility.Collapsed;
        return boolValue ? Visibility.Visible : falseValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility v && v == Visibility.Visible;
    }
}

/// <summary>
/// Inverse of BoolToVisibilityConverter.
/// false → Visible,  true → Collapsed
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = value is bool b && b;
        return boolValue ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility v && v == Visibility.Collapsed;
    }
}
