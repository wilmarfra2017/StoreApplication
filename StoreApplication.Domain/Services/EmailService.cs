using StoreApplication.Domain.Entities;
using StoreApplication.Domain.Ports;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Text;


namespace StoreApplication.Domain.Services;

[DomainService]
public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromAddress;

    public EmailService(IConfiguration configuration)
    {
        _smtpClient = new SmtpClient(configuration["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(configuration["EmailSettings:SmtpPort"]!),
            Credentials = new NetworkCredential(
                configuration["EmailSettings:SmtpUsername"],
                configuration["EmailSettings:SmtpPassword"]
            ),
            EnableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"]!)
        };

        _fromAddress = configuration["EmailSettings:FromAddress"]!;
    }

    public async Task SendOrderConfirmationEmailAsync(string to, Order order)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromAddress),
            Subject = "Confirmación de Pedido",
            IsBodyHtml = true
        };

        var bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine("<html><body>");
        bodyBuilder.AppendLine("<style>");
        bodyBuilder.AppendLine("table { width: 600px; border-collapse: collapse; margin: 20px auto; }"); 
        bodyBuilder.AppendLine("th, td { border: 1px solid #333; text-align: center; padding: 8px; }");
        bodyBuilder.AppendLine("th { background-color: #f2f2f2; }");
        bodyBuilder.AppendLine("tr:nth-child(even) {background-color: #f9f9f9;}");
        bodyBuilder.AppendLine("tr:hover {background-color: #f1f1f1;}");
        bodyBuilder.AppendLine("</style>");

        bodyBuilder.AppendLine($"<h1 style='text-align: center;'>Confirmación de Pedido #{order.Id}</h1>");
        bodyBuilder.AppendLine("<p style='text-align: center;'>Gracias por su pedido. Aquí están los detalles de su compra:</p>");
        bodyBuilder.AppendLine("<table>");
        bodyBuilder.AppendLine("<thead>");
        bodyBuilder.AppendLine("<tr><th>Producto</th><th>Cantidad</th><th>Precio Unitario</th><th>Total</th></tr>");
        bodyBuilder.AppendLine("</thead>");
        bodyBuilder.AppendLine("<tbody>");

        foreach (var detail in order.OrderDetails)
        {
            bodyBuilder.AppendLine("<tr>");
            bodyBuilder.AppendLine($"<td>{detail.ProductName}</td>");
            bodyBuilder.AppendLine($"<td>{detail.Quantity}</td>");
            bodyBuilder.AppendLine($"<td>${detail.UnitPrice}</td>");
            bodyBuilder.AppendLine($"<td>${detail.UnitPrice * detail.Quantity}</td>");
            bodyBuilder.AppendLine("</tr>");
        }

        bodyBuilder.AppendLine("</tbody>");
        bodyBuilder.AppendLine("</table>");
        bodyBuilder.AppendLine($"<p style='text-align: center;'>Total del pedido: <strong>${order.Total}</strong></p>");
        bodyBuilder.AppendLine("<p style='text-align: center;'>Esperamos que disfrutes tus productos. ¡Gracias por la compra!</p>");
        bodyBuilder.AppendLine("</body></html>");

        mailMessage.Body = bodyBuilder.ToString();
        mailMessage.To.Add(to);

        await _smtpClient.SendMailAsync(mailMessage);
    }
}
