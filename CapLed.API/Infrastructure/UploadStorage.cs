using Microsoft.Extensions.FileProviders;

namespace StockManager.API.Infrastructure;

public static class UploadStorage
{
    public const string EquipmentImagesRequestPath = "/images/equipments";

    public static string EquipmentImagesDirectory
    {
        get
        {
            var appServiceHome = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrWhiteSpace(appServiceHome))
            {
                return Path.Combine(appServiceHome, "data", "uploads", "images", "equipments");
            }

            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "equipments");
        }
    }

    public static void EnsureEquipmentImagesDirectory()
    {
        Directory.CreateDirectory(EquipmentImagesDirectory);
    }

    public static PhysicalFileProvider CreateEquipmentImagesProvider()
    {
        EnsureEquipmentImagesDirectory();
        return new PhysicalFileProvider(EquipmentImagesDirectory);
    }

    public static string BuildEquipmentImageUrl(string fileName)
        => $"{EquipmentImagesRequestPath}/{fileName}";

    public static string BuildEquipmentImagePhysicalPath(string imageUrl)
    {
        var safeFileName = Path.GetFileName(imageUrl.Replace('\\', '/'));
        return Path.Combine(EquipmentImagesDirectory, safeFileName);
    }

    public static string BuildSafeStoredFileName(string originalFileName)
    {
        var safeOriginalName = Path.GetFileName(originalFileName).Replace(" ", "_");
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            safeOriginalName = safeOriginalName.Replace(invalidChar, '_');
        }

        return $"{Guid.NewGuid()}_{safeOriginalName}";
    }
}
