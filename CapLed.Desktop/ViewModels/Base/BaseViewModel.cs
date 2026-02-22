using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CapLed.Desktop.ViewModels.Base;

/// <summary>
/// Base ViewModel that implements INotifyPropertyChanged.
/// All ViewModels in the application should inherit from this class.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises PropertyChanged for the calling property.
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets a backing field and raises PropertyChanged if the value changed.
    /// Returns true if the value was changed.
    /// Usage: SetProperty(ref _field, value);
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    // ── Loading / Error state (shared across all ViewModels) ──────────────────

    private bool _isLoading;
    private string? _errorMessage;
    private string? _successMessage;

    /// <summary>True while an async operation is in progress.</summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>Non-null when an error has occurred.</summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>Non-null after a successful operation.</summary>
    public string? SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    /// <summary>
    /// Helper: clear error + success, set loading = true.
    /// Call at the start of every async command.
    /// </summary>
    protected void BeginOperation()
    {
        ErrorMessage = null;
        SuccessMessage = null;
        IsLoading = true;
    }

    /// <summary>Helper: set loading = false.</summary>
    protected void EndOperation() => IsLoading = false;
}
