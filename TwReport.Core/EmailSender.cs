using Resend;
using TwReport.Core.Models;

namespace TwReport.Core;

public class EmailSender(IResend resend, ReportConfig config)
{
    private readonly IResend _resend = resend;
    private readonly ReportConfig _config = config;

    public async Task SendReportAsync(string subject, string body)
    {
        var message = new EmailMessage()
        {
            From = _config.Resend.FromEmail,
            To = _config.Resend.ToEmail,
            Subject = subject,
            TextBody = body
        };

        var response = await _resend.EmailSendAsync(message);

        if (response != null)
        {
            Console.WriteLine($"Email sent successfully. ID: {response.Content}");
        }
    }
}
