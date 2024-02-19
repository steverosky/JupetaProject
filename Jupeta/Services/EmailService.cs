using Jupeta.Controllers;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Jupeta.Services
{
    public class EmailConfiguration

    {
        public string From { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public interface IEmailService
    {
        void sendMail(string subj, string msg, string recipientEmail);
    }
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;

        public EmailService(EmailConfiguration emailConfig, ILogger<EmailService> logger, IConfiguration config)
        {
            _emailConfig = emailConfig;
            _logger = logger;
            _logger.LogInformation("Email Service called ");
            _config = config;
        }



        public void sendMail(string subj, string msg, string recipientEmail)
        {
            try
            {
                var fromAddress = new MailAddress(_emailConfig.From, _emailConfig.DisplayName);
                var toAddress = new MailAddress(recipientEmail);
                string fromPassword = _emailConfig.Password;
                string diplayname = _emailConfig.DisplayName;
                string subject = subj;
                string body = msg;
                var smtp = new SmtpClient
                {
                    Host = DotNetEnv.Env.GetString("mailcatcher"),
                    Port = 1025,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 50000

                };
                //_logger.LogInformation($"Sending mail to: {smtp.Host}");
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true

                })

                {
                    try
                    {
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                        //AlternateView imagelink = new AlternateView("cyberteqLogo.png", MediaTypeNames.Image.Jpeg)
                        //{ //var imageView =new AlternateView("cyberteqLogo.png", MediaTypeNames.Image.Jpeg);

                        //    ContentId = "image1",
                        //    TransferEncoding = TransferEncoding.Base64
                        //};

                        message.AlternateViews.Add(htmlView);
                        //message.AlternateViews.Add(imagelink);

                        smtp.Send(message);
                        _logger.LogInformation("Email sent successfully");
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        _logger.LogError(ex.FailedRecipient, this);
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable ||
                                status == SmtpStatusCode.TransactionFailed ||
                                status == SmtpStatusCode.ServiceNotAvailable ||
                                status == SmtpStatusCode.ServiceClosingTransmissionChannel ||
                                status == SmtpStatusCode.GeneralFailure)
                            {
                                Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                                Thread.Sleep(5000);
                                smtp.Send(message);
                            }
                            else
                            {
                                _logger.LogError("Failed to deliver message to {0}",
                                ex.InnerExceptions[i].FailedRecipient);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }

        }
    }
}
