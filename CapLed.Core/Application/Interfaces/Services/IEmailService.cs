namespace StockManager.Core.Application.Interfaces.Services;

public interface IEmailService
{
    /// <summary>
    /// Envoie un e-mail de confirmation de compte au client.
    /// </summary>
    Task SendConfirmationEmailAsync(string toEmail, string clientName, string confirmationLink);
}
