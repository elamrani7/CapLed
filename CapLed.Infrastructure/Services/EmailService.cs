using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockManager.Core.Application.Interfaces.Services;

namespace StockManager.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendConfirmationEmailAsync(string toEmail, string clientName, string confirmationLink)
    {
        var smtpHost = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
        var smtpUser = _config["Email:SmtpUser"] ?? "";
        var smtpPass = _config["Email:SmtpPass"] ?? "";
        var fromEmail = _config["Email:From"] ?? smtpUser;
        var fromName = _config["Email:FromName"] ?? "CapLed";

        var subject = "CapLed — Confirmez votre adresse e-mail";

        var htmlBody = $@"
<!DOCTYPE html>
<html lang=""fr"">
<head><meta charset=""UTF-8""></head>
<body style=""margin:0; padding:0; font-family: 'Segoe UI', Arial, sans-serif; background-color:#f4f6f9;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px; margin:30px auto; background:#ffffff; border-radius:12px; box-shadow:0 2px 12px rgba(0,0,0,0.08);"">
    <tr>
      <td style=""padding:40px 40px 20px 40px; text-align:center;"">
        <h1 style=""margin:0; font-size:28px; color:#1a1a2e; letter-spacing:-0.5px;"">
          ⚡ Cap<span style=""color:#e94560;"">Led</span>
        </h1>
        <p style=""color:#6c757d; font-size:14px; margin-top:5px;"">Solutions d'équipements industriels</p>
      </td>
    </tr>
    <tr>
      <td style=""padding:0 40px;"">
        <hr style=""border:none; border-top:1px solid #e9ecef; margin:0;"" />
      </td>
    </tr>
    <tr>
      <td style=""padding:30px 40px;"">
        <p style=""font-size:16px; color:#333; margin:0 0 20px 0;"">
          Bonjour <strong>{clientName}</strong>,
        </p>
        <p style=""font-size:15px; color:#555; line-height:1.6; margin:0 0 25px 0;"">
          Merci d'avoir créé un compte sur <strong>CapLed</strong>. 🚀<br/>
          Afin de sécuriser vos données et d'accéder à votre espace client,
          merci de confirmer votre adresse e-mail en cliquant sur le bouton ci-dessous :
        </p>
        <div style=""text-align:center; margin:30px 0;"">
          <a href=""{confirmationLink}"" 
             style=""display:inline-block; padding:14px 40px; background:linear-gradient(135deg, #e94560, #c23152); color:#ffffff; 
                    text-decoration:none; border-radius:8px; font-size:16px; font-weight:600; letter-spacing:0.5px;
                    box-shadow:0 4px 15px rgba(233,69,96,0.3);"">
            ✅ Confirmer mon compte
          </a>
        </div>
        <p style=""font-size:13px; color:#999; line-height:1.5; margin:25px 0 0 0;"">
          Ce lien est valide pendant <strong>24 heures</strong>.<br/>
          Si vous n'êtes pas à l'origine de cette demande, vous pouvez ignorer cet e-mail.
        </p>
      </td>
    </tr>
    <tr>
      <td style=""padding:0 40px;"">
        <hr style=""border:none; border-top:1px solid #e9ecef; margin:0;"" />
      </td>
    </tr>
    <tr>
      <td style=""padding:20px 40px 30px 40px; text-align:center;"">
        <p style=""font-size:12px; color:#adb5bd; margin:0;"">
          © {DateTime.UtcNow.Year} CapLed — Tous droits réservés.
        </p>
      </td>
    </tr>
  </table>
</body>
</html>";

        try
        {
            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            _logger.LogInformation("Confirmation email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to {Email}", toEmail);
            throw new Exception("Erreur SMTP lors de l'envoi de l'e-mail: " + ex.Message, ex);
        }
    }
}
