using System.Globalization;
using System.Windows.Data;

namespace CapLed.Desktop.Converters;

/// <summary>
/// Converts enum values (stored as strings from API) to human-readable French labels.
/// Usage in XAML:
///   Text="{Binding Condition, Converter={StaticResource EnumToStringConverter}}"
/// </summary>
[ValueConversion(typeof(string), typeof(string))]
public class EnumToStringConverter : IValueConverter
{
    // Mapping dictionary — extend as needed
    private static readonly Dictionary<string, string> Labels = new(StringComparer.OrdinalIgnoreCase)
    {
        // EquipmentCondition
        { "NEW",           "Neuf"          },
        { "USED",          "Occasion"      },
        { "DAMAGED",       "Endommagé"     },
        { "REPAIRING",     "En réparation" },
        // MovementType
        { "ENTRY",         "Entrée"        },
        { "EXIT",          "Sortie"        },
        // UserRole
        { "ADMIN",         "Administrateur" },
        { "STOCK_MANAGER", "Gestionnaire"  },
        // ContactStatus
        { "OPEN",          "Ouvert"        },
        { "CLOSED",        "Fermé"         },
    };

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && Labels.TryGetValue(str, out var label))
            return label;

        // Fallback for numeric enums or unknown values
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value?.ToString() ?? string.Empty;
}

/// <summary>
/// Checks if the bound string equals the CommandParameter string.
/// Used to highlight the active sidebar nav button.
/// Returns true (selected) when value == parameter.
/// </summary>
[ValueConversion(typeof(string), typeof(bool))]
public class StringEqualityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value?.ToString() == parameter?.ToString();

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => parameter?.ToString() ?? string.Empty;
}
