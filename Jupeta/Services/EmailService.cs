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
        public EmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
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
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 50000

                };
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
                        AlternateView imagelink = new AlternateView("cyberteqLogo.png", MediaTypeNames.Image.Jpeg)
                        { //var imageView =new AlternateView("cyberteqLogo.png", MediaTypeNames.Image.Jpeg);

                            ContentId = "image1",
                            TransferEncoding = TransferEncoding.Base64
                        };

                        message.AlternateViews.Add(htmlView);
                        message.AlternateViews.Add(imagelink);

                        smtp.Send(message);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        //_logger.LogError(ex.FailedRecipient, this);
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
                                //_logger.LogError("Failed to deliver message to {0}",
                                //ex.InnerExceptions[i].FailedRecipient);
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {
                //_logger.LogError(ex.Message, this);
            }

        }
    }
}
