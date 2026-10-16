using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace FincaNovaGestionLotes.Web.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
}

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SmtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var senderName = _config["Smtp:SenderName"] ?? "FincaNova";
        var senderEmail = _config["Smtp:SenderEmail"] ?? "";
        var smtpServer = _config["Smtp:Server"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_config["Smtp:Port"] ?? "587");
        var smtpPassword = _config["Smtp:Password"] ?? "";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Recuperación de Contraseña - FincaNova";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2 style='color: #2e7d32;'>Restablecer Contraseña</h2>
                    <p>Haz clic en el siguiente enlace para recuperar tu cuenta:</p>
                    <p><a href='{resetLink}' style='background: #2e7d32; color: white; padding: 10px 15px; text-decoration: none; border-radius: 5px; display: inline-block;'>Restablecer Contraseña</a></p>
                </div>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new MailKit.Net.Smtp.SmtpClient();
        await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(senderEmail, smtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}