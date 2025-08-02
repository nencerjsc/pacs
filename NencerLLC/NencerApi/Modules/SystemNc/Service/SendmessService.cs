using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NencerCore;
using NencerApi.Helpers;
using System.Net.Mail;
using System.Net;

namespace NencerApi.Modules.SystemNc.Service
{
    public class SendmessService
    {

        private readonly AppDbContext _context;

        public SendmessService(AppDbContext context)
        {
            _context = context;
        }

        //Hàm gửi mail chính
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string content)
        {
            try
            {
                var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == "sendmail_provider");
                if (setting == null || string.IsNullOrEmpty(setting.Value))
                {
                    throw new InvalidOperationException("Sendmail provider is not configured.");
                }

                if (setting.Value == "SendGrid")
                {
                    return await SendGrid(toEmail, subject, content);
                }
                else if (setting.Value == "SMTP")
                {
                    return await Smtp(toEmail, subject, content);
                }
                else
                {
                    throw new NotSupportedException($"Provider {setting.Value} is not supported.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SendGrid(string toEmail, string subject, string content)
        {
            try
            {
                var sendmess = await _context.Sendmesses.FirstOrDefaultAsync(s => s.Provider == "SendGrid" && s.Status == 1);
                if (sendmess == null || string.IsNullOrEmpty(sendmess.ApiKey))
                {
                    throw new InvalidOperationException("SendGrid configuration is missing or inactive.");
                }

                string from_email = Common.getSiteInfo("sendmail_email") ?? "no-reply@nencer.com";
                string name_email = Common.getSiteInfo("sendmail_name") ?? "Nencer Mail";

                var client = new SendGridClient(sendmess.ApiKey);
                var from = new EmailAddress(from_email, name_email);
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);

                var response = await client.SendEmailAsync(msg);

                return response.StatusCode == System.Net.HttpStatusCode.Accepted;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> Smtp(string toEmail, string subject, string content)
        {
            try
            {
                var sendmess = await _context.Sendmesses.FirstOrDefaultAsync(s => s.Provider == "SMTP" && s.Status == 1);
                if (sendmess == null || string.IsNullOrEmpty(sendmess.ApiKey) || string.IsNullOrEmpty(sendmess.ApiSecret) || string.IsNullOrEmpty(sendmess.Url) || string.IsNullOrEmpty(sendmess.Port))
                {
                    throw new InvalidOperationException("SMTP configuration is missing or incomplete.");
                }

                var smtpClient = new SmtpClient(sendmess.Url)
                {
                    Port = int.Parse(sendmess.Port),
                    Credentials = new NetworkCredential(sendmess.ApiKey, sendmess.ApiSecret),
                    EnableSsl = true
                };

                string from_email = Common.getSiteInfo("sendmail_email") ?? "no-reply@nencer.com";
                string name_email = Common.getSiteInfo("sendmail_name") ?? "Nencer Mail";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from_email, name_email),
                    Subject = subject,
                    Body = content,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }






    }
}
