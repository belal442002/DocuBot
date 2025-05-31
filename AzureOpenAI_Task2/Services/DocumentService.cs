
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using System.Text;
using Tesseract;

namespace AzureOpenAI_Task2.Services
{
    public class DocumentService : IDocumentService
    {
        public async Task<string> ExtractTextAsync(IFormFile file)
        {
            if(file == null || file.Length == 0) 
            {
                throw new Exception("No file provided");
            }

            var allowedExtensions = new string[] { ".png", "jpeg", "jpg", ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName);
            if(!allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception("The provided file is not supported");
            }

            // Read file into byte array
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            if (fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase)
             || fileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
             || fileExtension.Equals("jpg", StringComparison.OrdinalIgnoreCase))
            {
                return ExtractTextFromImage(fileBytes);
            }

            return ExtractTextFromPdf(fileBytes);
        }

        private string ExtractTextFromPdf(byte[] pdfBytes)
        {
            var text = new StringBuilder();

            using (var pdfReader = new PdfReader(new MemoryStream(pdfBytes)))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    var pageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);
                    text.Append(pageText);
                }
            }

            return text.ToString();
        }
        private string ExtractTextFromImage(byte[] imageBytes)
        {
            var tessdataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tessdata");
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            using (var img = Pix.LoadFromMemory(imageBytes))
            using (var page = engine.Process(img))
            {
                return page.GetText();
            }
        }
    }
}
