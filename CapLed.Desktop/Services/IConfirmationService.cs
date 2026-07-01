namespace CapLed.Desktop.Services;

/// <summary>
/// Protopyle for user confirmation prompts.
/// Decouples ViewModels from System.Windows.MessageBox.
/// </summary>
public interface IConfirmationService
{
    /// <summary>
    /// Asks a Yes/No question to the user.
    /// </summary>
    /// <param name="title">Window title.</param>
    /// <param name="message">Question text.</param>
    /// <returns>True if the user clicks Yes.</returns>
    bool Confirm(string title, string message);

    /// <summary>
    /// Shows an information alert.
    /// </summary>
    void ShowInfo(string title, string message);

    /// <summary>
    /// Shows an error alert.
    /// </summary>
    void ShowError(string title, string message);
}
