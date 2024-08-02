using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using ndisforms.Data.Models;
using ndisforms.Data.Providers;
using ndisforms.Data.Helpers;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
//using System.Net.Mail;

namespace ndisforms.Data.Services
{
    public interface IEmailAndNotificationService
    {        
        void SendEmailNewIR(int IrId, string baseurl);       
    }
    public class EmailAndNotificationService : IEmailAndNotificationService
    {        
        private readonly EmailOptions _EmailOptions;
        private readonly IIrDataProvider _irDataProvider;
        private readonly IEmailDataProvider _emailDataProvider;

        public EmailAndNotificationService(
            IIrDataProvider irDataProvider,
            IOptions<EmailOptions> emailOptions,
            IEmailDataProvider emailDataProvider)
        {           
            _EmailOptions = emailOptions.Value;
            _irDataProvider = irDataProvider;
            _emailDataProvider = emailDataProvider;
        }
        public void SendEmailNewIR(int IrId, string baseurl)
        {
            if (!_EmailOptions.Active)
            {
                return;
            }
            var Ir = _irDataProvider.GetIR(IrId);
            List<NotifyId> accessids = GetUsersToNotify("IR");
            if (accessids != null && Ir != null)
            {
                var subject = "New incident report - " + Ir.Report_number;
                var Emailmessage = "<br><br>New incident report has been created with reference number <strong>" + Ir.Report_number + "</strong>.";

                Emailmessage += "<br> Please use this number for all the future references.";

                var missionLink = GeneralHelpers.GenerateIrLink(Ir.Report_guid, baseurl);

                // Add the mission link to the email message
                Emailmessage += "<br><br>Click <a href=\"" + missionLink + "\">here</a> to view the incident report details.";

                Emailmessage += "<br><br><br><br>Thankyou<br>";
                foreach (var item in accessids)
                {
                    //int missionId = (int)item.MissionHeaderId;
                    var salutation = "Hi " + item.Name;
                    var messageHtml = salutation + Emailmessage;
                    string Errormsg = string.Empty;

                    var success = SendMail(subject, messageHtml, item.Email, string.Empty, out Errormsg);
                    if (success)
                    {
                        // Update email sent
                        //_viewDataService.UpdateEmailSendStatusInDistributionList(item.RecordId, true, string.Empty);
                        string emmsg = $"New incident report - Email notification sent to {item.Name} on {item.Email}.";
                       // _viewDataService.WriteToAuditLog("Email", emmsg, missionId, string.Empty);
                    }
                    else
                    {
                        //_viewDataService.UpdateEmailSendStatusInDistributionList(item.RecordId, false, Errormsg);
                        string emmsg = $"New incident report - Email notification not sent to  {item.Name} on {item.Email}. Error: {Errormsg}";
                       // _viewDataService.WriteToAuditLog("Email", emmsg, missionId, string.Empty);
                    }

                }
            }
        }

        public bool SendMail(string Subject, string messageBody, string ToAddress, string ToAddressDisplayName, out string statusmsg)
        {
            bool mailsent = false;
            statusmsg = string.Empty;

            var messagenew = new MimeMessage();
            messagenew.From.Add(new MailboxAddress("NDIS", _EmailOptions.SmtpUserName));
            messagenew.To.Add(new MailboxAddress(ToAddressDisplayName, ToAddress));
            messagenew.Subject = $"{Subject}";
            var builder = new BodyBuilder()
            {
                HtmlBody = messageBody
            };
            messagenew.Body = builder.ToMessageBody();
            try
            {
                using (var client = new SmtpClient())
                {
                    int smtpport = Convert.ToInt32(_EmailOptions.SmtpPort);
                    client.Connect(_EmailOptions.SmtpServer, smtpport, MailKit.Security.SecureSocketOptions.StartTls);
                    if (!string.IsNullOrEmpty(_EmailOptions.SmtpUserName) && !string.IsNullOrEmpty(_EmailOptions.SmtpPassword))
                    {
                        client.Authenticate(_EmailOptions.SmtpUserName, _EmailOptions.SmtpPassword);
                        client.Send(messagenew);
                        client.Disconnect(true);
                        mailsent = true;
                    }
                    else
                    {
                        statusmsg = "Smtp authentication not found. Please check settings.";
                    }
                }
            }
            catch (Exception ex)
            {
                statusmsg = ex.Message.ToString();
            }

            return mailsent;
        }

        private List<NotifyId> GetUsersToNotify(string emailtype)
        {
            List<NotifyId> accessids = new();

            var dl = _emailDataProvider.GetEmailIds(emailtype);
            foreach (var m in dl)
            {
                if (m.MailId != null)
                {
                    if (!accessids.Any(a => a.Uid == m.Id))
                    {
                        accessids.Add(new NotifyId
                        {
                            Uid = m.Id,
                            SendType = m.MailIdType,
                            Email = m.MailId,
                            Name = m.EmailName
                        });
                    }
                }
            }
            return accessids;
        }
               
    }

    public class NotifyId
    {
        public int Uid { get; set; }
        public string Name { get; set; }
        public string SendType { get; set; }
        public string Email { get; set; }
    }

}
