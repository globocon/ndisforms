using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using ndisforms.Data.Models;
using ndisforms.Data.Providers;
using ndisforms.Data.Helpers;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using System.Text;

//using System.Net.Mail;

namespace ndisforms.Data.Services
{
    public interface IEmailAndNotificationService
    {
        Task SendEmailNewIRAsync(int IrId, string baseurl);
    }
    public class EmailAndNotificationService : IEmailAndNotificationService
    {        
        private readonly EmailOptions _EmailOptions;
        private readonly IIrDataProvider _irDataProvider;
        private readonly IEmailDataProvider _emailDataProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmailAndNotificationService(
            IIrDataProvider irDataProvider,
            IOptions<EmailOptions> emailOptions,
            IEmailDataProvider emailDataProvider,
            IWebHostEnvironment webHostEnvironment)
        {           
            _EmailOptions = emailOptions.Value;
            _irDataProvider = irDataProvider;
            _emailDataProvider = emailDataProvider;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task SendEmailNewIRAsync(int IrId, string baseurl)
        {
            if (!_EmailOptions.Active)
            {
                return;
            }
            
            var Ir = _irDataProvider.GetIR(IrId);

            //pdf generation
            var htmlContent = GenerateHtmlContent(Ir, baseurl);
            var pdfService = new PdfGenerator();
            //var url = "https://localhost:7133/PdfForms/IncidentReportForm";
            
            var pdfFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", "Pdf");
            var outputFilePath = Path.Combine(pdfFolderPath, "incident_report.pdf");
            //await pdfService.CreatePdfFromUrlAsync(url, outputFilePath);
            await pdfService.GeneratePdfAsync(htmlContent, outputFilePath);
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
        private string GenerateHtmlContent(IR_Header Ir, string baseurl)
        {
            var sb = new StringBuilder();

            // Add the static HTML structure
            sb.Append("<html><head><style>");
            // Add any CSS styling here if needed
            sb.Append("body { font-family: Arial, sans-serif; }");
            sb.Append(".card { border: 1px solid #ccc; border-radius: 4px; margin-bottom: 20px; padding: 15px; }");
            sb.Append(".card-header { font-weight: bold; }");
            sb.Append("html { font-size: 14px; }");
            sb.Append("@media (min-width: 768px) { html { font-size: 16px; } }");
            sb.Append(".btn:focus, .btn:active:focus, .btn-link.nav-link:focus, .form-control:focus, .form-check-input:focus { box-shadow: 0 0 0 0.1rem white, 0 0 0 0.25rem #258cfb; }");
            sb.Append("html { position: relative; min-height: 100%; }");
            sb.Append("body { margin-bottom: 60px; }");
            sb.Append("#loader, #loader-p { display: none; position: fixed; top: 0; left: 0; right: 0; bottom: 0; width: 100%; background: rgba(0,0,0,0.5) url('../images/loading.gif') no-repeat center center; z-index: 1000; }");
            sb.Append("</style></head><body>");

            sb.Append("<div class='row my-3 my-md-2'>");
            sb.Append("    <div class='col-md-10'>");
            sb.Append("        <p><h2>Incident Report Form</h2>");
            sb.Append("        <h4 style='color: darkred;'>My Choice Support Care</h4></p>");
            sb.Append("    </div>");
            sb.Append("    <div class='col-md-2 pr-2'>");
            sb.Append($"        <img class='w-100' src='{baseurl}images/logo.png' />");
            sb.Append("    </div>");
            sb.Append("</div>");

            sb.Append("<form method='post' enctype='multipart/form-data' id='ndisir-form'>");
            sb.Append("<input type='hidden' name='NewIRHeader.Report_guid' value='" + Ir.Report_guid + "' />");
            // Type of incident
            sb.Append("<div class='row mb-3'>");
            sb.Append("    <div class='col-md-12'>");
            sb.Append("        <div class='card'>");
            sb.Append("            <h5 class='card-header'>Type of incident</h5>");
            sb.Append("            <div class='card-body'>");
            sb.Append("                <div class='row g-3'>");
            sb.Append("                    <div class='col-md-12'>");
            sb.Append("                        <div class='form-check-inline' style='width:350px'>");
            sb.Append("                            <label class='form-check-label' style='width:350px'> Is it a reportable incident?</label>");                          

            sb.Append("                            <input type='radio' class='form-check-input' value='1' name='NewIRHeader_Toi_reportable_incident' " + (Ir.Toi_reportable_incident ? "checked" : "") + " />");
            sb.Append("                            <label class='form-check-label' for='NewIRHeader_Toi_reportable_incident_yes'> Yes</label>");
                        
            sb.Append("                            <input type='radio' class='form-check-input' value='0' id='NewIRHeader_Toi_reportable_incident_no' name='NewIRHeader_Toi_reportable_incident' " + (!Ir.Toi_reportable_incident ? "checked" : "") + " />");
            sb.Append("                            <label class='form-check-label' for='NewIRHeader_Toi_reportable_incident_no'> No</label>");
            sb.Append("                        </div>");
            sb.Append("                    </div>");
            sb.Append("                </div>");
            sb.Append("            </div>");
            sb.Append("        </div>");
            sb.Append("    </div>");
            sb.Append("</div>");

            // Staff member details
            sb.Append("<div style='margin-bottom: 20px;'>");
            sb.Append("    <div style='border: 1px solid #ccc; border-radius: 4px; padding: 15px;'>");
            sb.Append("        <div style='margin-bottom: 10px;'>");
            sb.Append("            <label style='display: block; margin-bottom: 5px; width:350px'>Names of witnesses: (if applicable)</label>");
            sb.Append("            <input type='text' name='NewIRHeader_Nosmpr_name_of_witness' value='" + Ir.Nosmpr_name_of_witness + "' style='width: 100%; box-sizing: border-box;' />");
            sb.Append("        </div>");
            sb.Append("        <div style='margin-bottom: 10px;'>");
            sb.Append("            <label style='display: block; margin-bottom: 5px; width:350px'>Report relates to:</label>");
            sb.Append("            ");
            sb.Append("                <label style='display: block;'><input type='checkbox' name='NewIRHeader_Nosmpr_rpt_rltd_hazard' " + (Ir.Nosmpr_rpt_rltd_hazard ? "checked" : "") + " /> Hazard</label>");
            sb.Append("                <label style='display: block;'><input type='checkbox' name='NewIRHeader_Nosmpr_rpt_rltd_nearmiss' " + (Ir.Nosmpr_rpt_rltd_nearmiss ? "checked" : "") + " /> Near-miss</label>");
            sb.Append("                <label style='display: block;'><input type='checkbox' name='NewIRHeader_Nosmpr_rpt_rltd_incident' " + (Ir.Nosmpr_rpt_rltd_incident ? "checked" : "") + " /> Incident</label>");
            sb.Append("                <label style='display: block;'><input type='checkbox' name='NewIRHeader_Nosmpr_rpt_rltd_concernchange' " + (Ir.Nosmpr_rpt_rltd_concernchange ? "checked" : "") + " /> Concern/Change</label>");
            sb.Append("            ");
            sb.Append("        </div>");
            sb.Append("        <div style='margin-bottom: 10px;'>");
            sb.Append("            <label style='display: block; margin-bottom: 5px;width:450px'>Date and time of when issue/incident occurred or was noticed:</label>");
            sb.Append("            <input type='text' name='NewIRHeader_Nosmpr_rpt_dtm' value='" + Ir.Nosmpr_rpt_dtm + "' style='width: 100%; box-sizing: border-box;' />");
            sb.Append("        </div>");
            sb.Append("        <div style='margin-bottom: 10px;'>");
            sb.Append("            <label style='display: block; margin-bottom: 5px;'>Location / Address:</label>");
            sb.Append("            <input type='text' name='NewIRHeader_Nosmpr_rpt_location' value='" + Ir.Nosmpr_rpt_location + "' style='width: 100%; box-sizing: border-box;' />");
            sb.Append("        </div>");
            sb.Append("        <div style='margin-bottom: 10px;'>");
            sb.Append("            <label style='display: block; margin-bottom: 5px;'>Name of Client:</label>");
            sb.Append("            <input type='text' name='NewIRHeader_Nosmpr_rpt_rltd_nameofclient' value='" + Ir.Nosmpr_rpt_rltd_nameofclient + "' style='width: 100%; box-sizing: border-box;' />");
            sb.Append("        </div>");
            sb.Append("    </div>");
            sb.Append("</div>");

            // Description of issue being reported
            sb.Append("<div class='row mb-3'>");
            sb.Append("    <div class='col-md-12'>");
            sb.Append("        <div class='card'>");
            sb.Append("            <h5 class='card-header'>Description of issue being reported: (sketch if required)</h5>");
            sb.Append("            <div class='card-body'>");
            sb.Append("                <div class='col-md-12'>");
            sb.Append("                    <div class='form-check-inline w-100'>");
            sb.Append("                        <textarea class='w-100' rows='5' maxlength='500' name='NewIRHeader_Doibr_text' style='width: 100%; box-sizing: border-box;'>" + Ir.Doibr_text + "</textarea>");
            sb.Append("                    </div>");
            sb.Append("                </div>");
            sb.Append("            </div>");
            sb.Append("        </div>");
            sb.Append("    </div>");
            sb.Append("</div>");

            sb.Append("<div class='row mb-3'>");
            sb.Append("    <div class='col-md-4'>");
            sb.Append("        <button type='button' id='btn_submitreport' class='btn btn-primary'> Submit Report</button>");
            sb.Append("    </div>");
            sb.Append("</div>");

            sb.Append("</form>");
            sb.Append("</body></html>");

            return sb.ToString();
        }


        public bool SendMail(string Subject, string messageBody, string ToAddress, string ToAddressDisplayName, out string statusmsg)
        {
            bool mailsent = false;
            statusmsg = string.Empty;
            var pdfFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", "Pdf");
            var outputFilePath = Path.Combine(pdfFolderPath, "incident_report.pdf");
            var messagenew = new MimeMessage();
            //messagenew.From.Add(new MailboxAddress("NDIS", _EmailOptions.SmtpUserName));
            //messagenew.To.Add(new MailboxAddress(ToAddressDisplayName, ToAddress));
            messagenew.From.Add(new MailboxAddress("NDIS", "hello@mychoicesupportcare.com.au"));
            messagenew.To.Add(new MailboxAddress("MyChoice SupportCare", "hello@mychoicesupportcare.com.au"));
            messagenew.Subject = $"{Subject}";
            var builder = new BodyBuilder()
            {
                HtmlBody = messageBody
            };
       
            builder.Attachments.Add(outputFilePath);
            messagenew.Body = builder.ToMessageBody();
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("localhost", 25, MailKit.Security.SecureSocketOptions.None);
                    client.Send(messagenew);
                    client.Disconnect(true);
                }
                //using (var client = new SmtpClient())
                //{
                //    int smtpport = Convert.ToInt32(_EmailOptions.SmtpPort);
                //    client.Connect(_EmailOptions.SmtpServer, smtpport, MailKit.Security.SecureSocketOptions.SslOnConnect);
                //    if (!string.IsNullOrEmpty(_EmailOptions.SmtpUserName) && !string.IsNullOrEmpty(_EmailOptions.SmtpPassword))
                //    {
                //        client.Authenticate(_EmailOptions.SmtpUserName, _EmailOptions.SmtpPassword);
                //        client.Send(messagenew);
                //        client.Disconnect(true);
                //        mailsent = true;
                //    }
                //    else
                //    {
                //        statusmsg = "Smtp authentication not found. Please check settings.";
                //    }
                //}
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
