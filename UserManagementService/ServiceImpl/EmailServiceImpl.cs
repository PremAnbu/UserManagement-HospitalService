using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using UserManagementService.Interface;
using UserManagementService.Dto.RequestDto;
using UserManagementService.GlobleCustomExceptionHandler;
using System;

namespace UserManagementService.ServiceImpl
{
    public class EmailServiceImpl : IEmail
    {
        private readonly EmailSettings _emailSettings;

        public EmailServiceImpl(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> EmailSenderAsync(string to, string subject, string htmlMessage)
        {
            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                    Console.WriteLine("step 1");
                    using (var mailMessage = new MailMessage {
                        From = new MailAddress(_emailSettings.FromEmail),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    })
                    {
                        mailMessage.To.Add(to);
                        await client.SendMailAsync(mailMessage);
                        return true;
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                throw new EmailSendingException("SMTP error occurred while sending   email", smtpEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                throw new EmailSendingException("Invalid operation occurred while sending email", invalidOpEx);
            }
            catch (Exception ex)
            {
                throw new EmailSendingException("An error occurred while sending email", ex);
            }
        }
    }
}
