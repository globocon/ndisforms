using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ndisforms.Data.Services;
using ndisforms.Pages.PdfForms;
using ndisforms.Data.Helpers;
using ndisforms.Data.Models;

namespace ndisforms.Pages
{
    public class ViewReportsModel : PageModel
    {
        private readonly IViewDataService _viewDataService;
        private readonly IEmailAndNotificationService _emailAndNotificationService;
        public ViewReportsModel(IViewDataService viewDataService, IEmailAndNotificationService emailAndNotificationService)
        {
            _viewDataService = viewDataService;
            _emailAndNotificationService = emailAndNotificationService;
        }

        public void OnGet()
        {
        }
        public IActionResult OnGetViewAllReports()
        {
            var result = _viewDataService.GetDataIncidentReports;
            return new JsonResult(result);
        }
        public IActionResult OnPostSendEmail(int id)
        {
            var baseUrl = HttpContext.Request.BaseUrl();
            _emailAndNotificationService.SendEmailNewIRAsync(id,baseUrl);
            // Implement your email sending logic here
            return new JsonResult(new { success = true });
        }
    }
}
