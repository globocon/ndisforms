using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ndisforms.Data.Helpers;
using ndisforms.Data.Models;
using ndisforms.Data.Services;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.IO;
using iText.Html2pdf;

namespace ndisforms.Pages.PdfForms
{
    public class IncidentReportFormModel : PageModel
    {
        private readonly IViewDataService _viewDataService;
        private readonly IEmailAndNotificationService _emailAndNotificationService;
        private readonly ILogger<IncidentReportFormModel> _logger;

        public IncidentReportFormModel(IViewDataService viewDataService,
           IEmailAndNotificationService emailAndNotificationService,
           ILogger<IncidentReportFormModel> logger)
        {
            _viewDataService = viewDataService;
            _emailAndNotificationService = emailAndNotificationService;
            _logger = logger;
        }

        [BindProperty]
        public IR_Header NewIRHeader { get; set; }
        public IActionResult OnGet()
        {
            NewIRHeader = new IR_Header
            {
                Report_guid = Guid.NewGuid(),                
            };

            return Page();
        }

        public IActionResult OnPostSubmitIncidentReport(IR_Header NewIRHeader)
        {
            var status = false;
            var message = "Error";
            int newirid = 0;
            var irNumber = "";
            var baseUrl="";
            try
            {
                var res = _viewDataService.SaveNewIR(NewIRHeader);

                if (res.Id > 0)
                {
                     baseUrl = HttpContext.Request.BaseUrl();
                    _emailAndNotificationService.SendEmailNewIRAsync(res.Id, baseUrl);
                    // Implement your email sending logic here
                    return new JsonResult(new { success = true });
                    status = true;
                    message = $"Incident Report created successfully with refrence number {res.Report_number}.";
                    newirid = res.Id;
                    irNumber = res.Report_number;
                    
                    try
                    {
                        // Try sending mail
                         baseUrl = HttpContext.Request.BaseUrl();
                        _emailAndNotificationService.SendEmailNewIRAsync(newirid, baseUrl);
                    }
                    catch (Exception er)
                    {
                        // throw;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                message = "Error: " + ex.Message;
            }
           

            return new JsonResult(new { status = status, message = message, newirid = newirid, irNumber = irNumber });
        }

        public byte[] ConvertHtmlToPdf(string htmlContent)
        {
            using (var memoryStream = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(htmlContent, memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
