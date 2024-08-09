using iText.Html2pdf;
using System;
using System.IO;
using System.Threading.Tasks;

public class PdfGenerator
{
    public async Task GeneratePdfAsync(string htmlContent, string outputFilePath)
    {
        try
        {
            if (string.IsNullOrEmpty(outputFilePath))
            {
                throw new ArgumentException("Output file path cannot be null or empty.", nameof(outputFilePath));
            }

            var outputDir = Path.GetDirectoryName(outputFilePath);
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
          
        
                // Convert HTML to PDF directly using the output stream
                HtmlConverter.ConvertToPdf(htmlContent, outputStream);
            }

            Console.WriteLine("PDF generated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
