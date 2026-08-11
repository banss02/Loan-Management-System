using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using LoanAPI.DTOs;

namespace LoanAPI.Services
{
    public class DocumentExtractionService
    {
        public async Task<ExtractedCustomerDataDto> ExtractCustomerData(IFormFile file)
        {
            var text = await ReadFileText(file);

            Console.WriteLine("===== PDF TEXT =====");
            Console.WriteLine(text);
            Console.WriteLine("====================");

            var dto = new ExtractedCustomerDataDto();

            dto.FullName = GetValue(text,
                @"Full\s*Name\s*:\s*(.*?)\s*(?=Date\s*Of\s*Birth|DOB|Email|Phone|Company|PAN|Aadhaar|Guardian|Address|Bank|Account|IFSC|$)");

            dto.DateOfBirth = ParseDate(GetValue(text,
                @"(?:Date\s*Of\s*Birth|DOB)\s*:\s*(.*?)\s*(?=Email|Phone|Company|PAN|Aadhaar|Guardian|Address|Bank|Account|IFSC|$)"));

            dto.Email = GetValue(text,
                @"Email\s*:\s*(.*?)\s*(?=Phone|Company|PAN|Aadhaar|Guardian|Address|Bank|Account|IFSC|$)");

            dto.Phone = GetValue(text,
                @"Phone\s*:\s*(.*?)\s*(?=Company|PAN|Aadhaar|Guardian|Address|Bank|Account|IFSC|$)");

            dto.CompanyName = GetValue(text,
                @"Company\s*Name\s*:\s*(.*?)\s*(?=PAN|Aadhaar|Guardian|Address|Bank|Account|IFSC|$)");

            dto.PANNumber = GetValue(text,
                @"PAN\s*:\s*(.*?)\s*(?=Aadhaar|Guardian|Address|Bank|Account|IFSC|$)");

            dto.AadhaarNumber = GetValue(text,
                @"Aadhaar\s*Number\s*:\s*(.*?)\s*(?=Guardian|Address|Bank|Account|IFSC|$)");

            dto.GuardianName = GetValue(text,
                @"Guardian\s*Name\s*:\s*(.*?)\s*(?=Address|Bank|Account|IFSC|$)");

            dto.Address = GetValue(text,
                @"Address\s*:\s*(.*?)\s*(?=Bank|Account|IFSC|$)");

            dto.BankName = GetValue(text,
                @"Bank\s*Name\s*:\s*(.*?)\s*(?=Account|IFSC|$)");

            dto.AccountNumber = GetValue(text,
                @"Account\s*Number\s*:\s*(.*?)\s*(?=IFSC|$)");

            dto.IFSCCode = GetValue(text,
                @"IFSC\s*:\s*(.*)");

            dto.FieldsFound = new[]
            {
                dto.FullName,
                dto.Email,
                dto.Phone,
                dto.CompanyName,
                dto.PANNumber,
                dto.AadhaarNumber,
                dto.GuardianName,
                dto.Address,
                dto.BankName,
                dto.AccountNumber,
                dto.IFSCCode
            }.Count(x => !string.IsNullOrWhiteSpace(x));

            return dto;
        }

        private static string? GetValue(string text, string pattern)
        {
            var match = Regex.Match(
                text,
                pattern,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (!match.Success)
                return null;

            return match.Groups[1].Value.Trim();
        }

        private static DateOnly? ParseDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateOnly.TryParse(value, out var date))
                return date;

            return null;
        }

        private async Task<string> ReadFileText(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".txt")
            {
                using var reader = new StreamReader(file.OpenReadStream());
                return await reader.ReadToEndAsync();
            }

            if (extension == ".pdf")
            {
                using var stream = file.OpenReadStream();
                using var memory = new MemoryStream();

                await stream.CopyToAsync(memory);

                using var pdf = PdfDocument.Open(memory.ToArray());

                var builder = new StringBuilder();

                foreach (var page in pdf.GetPages())
                {
                    builder.Append(' ');
                    builder.Append(page.Text);
                }

                return Regex.Replace(builder.ToString(), @"\s+", " ").Trim();
            }

            return string.Empty;
        }
    }
}