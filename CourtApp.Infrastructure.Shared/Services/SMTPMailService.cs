using CourtApp.Application.DTOs.Mail;
using CourtApp.Application.DTOs.Settings;
using CourtApp.Application.Interfaces.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.Shared.Services
{
    public class SMTPMailService : IMailService
    {
        public MailSettings _mailSettings { get; }
        public ILogger<SMTPMailService> _logger { get; }

        public SMTPMailService(IOptions<MailSettings> mailSettings, ILogger<SMTPMailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                var fromAddress = new MailAddress(_mailSettings.From, _mailSettings.DisplayName);
                var toAddress = new MailAddress(request.To);
                var smtpClient = new SmtpClient(_mailSettings.Host)
                {
                    Host = _mailSettings.Host,
                    Port = _mailSettings.Port,
                    EnableSsl = _mailSettings.EnableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_mailSettings.UserName, _mailSettings.Password),
                    Timeout = 300000
                };
                using var mailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = string.IsNullOrWhiteSpace(request.Subject) ? "" : request.Subject.Trim(),
                    Body = request.Body,
                    IsBodyHtml = true,
                };
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                _logger.LogError(ex.Message, ex);
            }

        }
    }
}