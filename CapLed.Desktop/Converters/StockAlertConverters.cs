using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using StockManager.Core.Domain.Enums;

namespace CapLed.Desktop.Converters;

// File: StockAlertConverters.cs
public class StockAlertToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is StockAlertLevel level)
        {
            return level switch
            {
                StockAlertLevel.OUT_OF_STOCK => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")), // Red-500
                StockAlertLevel.CRITICAL     => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F97316")), // Orange-500
                StockAlertLevel.WARNING      => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EAB308")), // Yellow-500
                _                            => Brushes.Transparent
            };
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class StockAlertToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is StockAlertLevel level)
        {
            return level switch
            {
                StockAlertLevel.OUT_OF_STOCK => "🛑",
                StockAlertLevel.CRITICAL     => "❗",
                StockAlertLevel.WARNING      => "⚠️",
                _                            => string.Empty
            };
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class StockAlertToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is StockAlertLevel level)
        {
            return level switch
            {
                StockAlertLevel.OUT_OF_STOCK => "Rupture de stock",
                StockAlertLevel.CRITICAL     => "Stock Critique",
                StockAlertLevel.WARNING      => "Stock Faible",
                _                            => "Normal"
            };
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
