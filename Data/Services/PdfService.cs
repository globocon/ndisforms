using System;
using System.Threading.Tasks;
namespace ndisforms.Data.Services
{

    public class PdfService
    {
        private readonly HtmlFetcher _htmlFetcher;
        private readonly PdfGenerator _pdfGenerator;

        public PdfService()
        {
            _htmlFetcher = new HtmlFetcher();
            _pdfGenerator = new PdfGenerator();
        }

        public async Task CreatePdfFromUrlAsync(string url, string outputFilePath)
        {
            // Fetch HTML content from the URL
            var htmlContent = await _htmlFetcher.FetchHtmlAsync(url);

            // Convert HTML content to PDF
            await _pdfGenerator.GeneratePdfAsync(htmlContent, outputFilePath);
        }
    }
}
