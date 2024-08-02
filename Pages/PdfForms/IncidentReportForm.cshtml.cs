using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ndisforms.Data.Helpers;
using ndisforms.Data.Models;
using ndisforms.Data.Services;

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
            try
            {
                var res = _viewDataService.SaveNewIR(NewIRHeader);

                if (res.Id > 0)
                {
                    status = true;
                    message = $"Incident Report created successfully with refrence number {res.Report_number}.";
                    newirid = res.Id;
                    irNumber = res.Report_number;
                    
                    try
                    {
                        // Try sending mail
                        var baseUrl = HttpContext.Request.BaseUrl();
                        _emailAndNotificationService.SendEmailNewIR(newirid, baseUrl);
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
    }
}
