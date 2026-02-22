namespace CapLed.Desktop.Models;

/// <summary>
/// Mirrors EquipmentListItemDto — used in DataGrids and lists.
/// </summary>
public class EquipmentListItemModel
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;  // "NEW" or "USED"
    public int Quantity { get; set; }
}

/// <summary>
/// Mirrors EquipmentReadDto — used in detail/edit views.
/// </summary>
public class EquipmentDetailModel : EquipmentListItemModel
{
    public string? Description { get; set; }
    public int CategoryId { get; set; } // Added to support selection in Edit form
    public int MinThreshold { get; set; }
    public bool IsPublished { get; set; }
    public List<PhotoModel> Photos { get; set; } = new();
}

/// <summary>
/// Mirrors EquipmentCreateDto / EquipmentUpdateDto — used in create/edit forms.
/// </summary>
public class EquipmentEditModel
{
    public string Reference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string Condition { get; set; } = "NEW";
    public int MinThreshold { get; set; }
    public bool IsPublished { get; set; }
}

/// <summary>
/// Mirrors PhotoDto.
/// </summary>
public class PhotoModel
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}
