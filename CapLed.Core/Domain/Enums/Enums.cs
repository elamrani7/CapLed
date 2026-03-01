using System.Text.Json.Serialization;

namespace StockManager.Core.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EquipmentCondition
{
    NEW,
    USED,
    DAMAGED,
    REPAIRING
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    ADMIN,
    STOCK_MANAGER
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MovementType
{
    ENTRY,
    EXIT
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContactStatus
{
    NEW,
    OPEN,
    CLOSED
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StockAlertLevel
{
    NONE,
    WARNING,
    CRITICAL,
    OUT_OF_STOCK
}

public static class StockAlertHelper
{
    public static StockAlertLevel GetAlertLevel(int quantity)
    {
        if (quantity <= 0) return StockAlertLevel.OUT_OF_STOCK;
        if (quantity <= 3) return StockAlertLevel.CRITICAL;
        if (quantity <= 5) return StockAlertLevel.WARNING;
        return StockAlertLevel.NONE;
    }
}

