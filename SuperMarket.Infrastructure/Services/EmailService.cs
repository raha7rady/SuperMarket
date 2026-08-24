using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SuperMarket.Application.Common.Interfaces;

namespace SuperMarket.Infrastructure.Services;

public sealed class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> options)
    {
        _settings = options?.Value
            ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true)
    {
        await SendEmailAsync(to, subject, body, _settings.From, isHtml);
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        string? from,
        bool isHtml = true)
    {
        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("Recipient email is required.", nameof(to));

        using var message = new MailMessage
        {
            From = new MailAddress(from ?? _settings.From),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        message.To.Add(to);

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.User, _settings.Password)
        };

        await client.SendMailAsync(message);
    }
}