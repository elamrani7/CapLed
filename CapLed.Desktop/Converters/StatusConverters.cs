using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CapLed.Desktop.Converters;

/// <summary>
/// Converts a string to Visibility. Visible if not null or empty.
/// </summary>
public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Returns a background brush based on Equipment condition string.
/// </summary>
public class ConditionToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? condition = value?.ToString();
        return condition switch
        {
            "NEW" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E")),      // Success Green
            "USED" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),     // Info Blue
            "DAMAGED" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")),  // Danger Red
            "REPAIRING" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")),// Warning Orange
            _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"))         // Slate Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
