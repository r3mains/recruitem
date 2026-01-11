using backend.Services.IServices;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace backend.Services;

public class PdfGenerationService(IWebHostEnvironment environment, ILogger<PdfGenerationService> logger) : IPdfGenerationService
{
  private readonly IWebHostEnvironment _environment = environment;
  private readonly ILogger<PdfGenerationService> _logger = logger;

  public async Task<string> GenerateOfferLetterPdfAsync(
    string candidateName,
    string jobTitle,
    string companyName,
    string? companyAddress,
    decimal salary,
    DateTime joiningDate,
    DateTime offerDate,
    string? benefits,
    string? additionalTerms,
    string? signatoryName,
    string? signatoryDesignation)
  {
    return await Task.Run(() =>
    {
      try
      {
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads", "offerletters");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"OfferLetter_{candidateName.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          var document = new Document(PageSize.A4, 50, 50, 50, 50);
          var writer = PdfWriter.GetInstance(document, stream);
          
          document.Open();

          // Fonts
          var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(0, 0, 0));
          var headingFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, new BaseColor(0, 0, 0));
          var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, new BaseColor(0, 0, 0));
          var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(0, 0, 0));

          // Company Header
          var headerParagraph = new Paragraph($"{companyName}\n", titleFont)
          {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 5
          };
          document.Add(headerParagraph);

          if (!string.IsNullOrEmpty(companyAddress))
          {
            var addressParagraph = new Paragraph(companyAddress, normalFont)
            {
              Alignment = Element.ALIGN_CENTER,
              SpacingAfter = 20
            };
            document.Add(addressParagraph);
          }

          // Horizontal line
          var line = new Paragraph("_____________________________________________________________________________")
          {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 20
          };
          document.Add(line);

          // Title
          var offerTitleParagraph = new Paragraph("OFFER LETTER", headingFont)
          {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 20
          };
          document.Add(offerTitleParagraph);

          // Date
          var dateParagraph = new Paragraph($"Date: {offerDate:MMMM dd, yyyy}", normalFont)
          {
            SpacingAfter = 15
          };
          document.Add(dateParagraph);

          // Addressee
          var addresseeParagraph = new Paragraph($"Dear {candidateName},", normalFont)
          {
            SpacingAfter = 15
          };
          document.Add(addresseeParagraph);

          // Introduction
          var introParagraph = new Paragraph(
            $"We are pleased to offer you the position of {jobTitle} at {companyName}. " +
            "We believe that your skills and experience will be a valuable asset to our organization.",
            normalFont
          )
          {
            Alignment = Element.ALIGN_JUSTIFIED,
            SpacingAfter = 15
          };
          document.Add(introParagraph);

          // Terms and Conditions Heading
          var termsHeading = new Paragraph("Terms and Conditions:", headingFont)
          {
            SpacingAfter = 10
          };
          document.Add(termsHeading);

          // Position
          var positionChunk = new Chunk("Position: ", boldFont);
          var positionValue = new Chunk($"{jobTitle}\n\n", normalFont);
          var positionParagraph = new Paragraph();
          positionParagraph.Add(positionChunk);
          positionParagraph.Add(positionValue);
          document.Add(positionParagraph);

          // Salary
          var salaryChunk = new Chunk("Annual Compensation: ", boldFont);
          var salaryValue = new Chunk($"${salary:N2}\n\n", normalFont);
          var salaryParagraph = new Paragraph();
          salaryParagraph.Add(salaryChunk);
          salaryParagraph.Add(salaryValue);
          document.Add(salaryParagraph);

          // Joining Date
          var joiningChunk = new Chunk("Expected Joining Date: ", boldFont);
          var joiningValue = new Chunk($"{joiningDate:MMMM dd, yyyy}\n\n", normalFont);
          var joiningParagraph = new Paragraph();
          joiningParagraph.Add(joiningChunk);
          joiningParagraph.Add(joiningValue);
          document.Add(joiningParagraph);

          // Benefits
          if (!string.IsNullOrEmpty(benefits))
          {
            var benefitsChunk = new Chunk("Benefits: ", boldFont);
            var benefitsValue = new Chunk($"{benefits}\n\n", normalFont);
            var benefitsParagraph = new Paragraph();
            benefitsParagraph.Add(benefitsChunk);
            benefitsParagraph.Add(benefitsValue);
            document.Add(benefitsParagraph);
          }

          // Additional Terms
          if (!string.IsNullOrEmpty(additionalTerms))
          {
            var additionalChunk = new Chunk("Additional Terms: ", boldFont);
            var additionalValue = new Chunk($"{additionalTerms}\n\n", normalFont);
            var additionalParagraph = new Paragraph();
            additionalParagraph.Add(additionalChunk);
            additionalParagraph.Add(additionalValue);
            document.Add(additionalParagraph);
          }

          // Closing
          var closingParagraph = new Paragraph(
            "Please confirm your acceptance of this offer by signing and returning a copy of this letter. " +
            "We look forward to welcoming you to our team.",
            normalFont
          )
          {
            Alignment = Element.ALIGN_JUSTIFIED,
            SpacingAfter = 30,
            SpacingBefore = 15
          };
          document.Add(closingParagraph);

          // Signature section
          var signatureParagraph = new Paragraph("Sincerely,\n\n\n", normalFont)
          {
            SpacingAfter = 5
          };
          document.Add(signatureParagraph);

          if (!string.IsNullOrEmpty(signatoryName))
          {
            var signatoryNameParagraph = new Paragraph(signatoryName, boldFont);
            document.Add(signatoryNameParagraph);
          }

          if (!string.IsNullOrEmpty(signatoryDesignation))
          {
            var signatoryDesignationParagraph = new Paragraph(signatoryDesignation, normalFont)
            {
              SpacingAfter = 30
            };
            document.Add(signatoryDesignationParagraph);
          }

          // Acceptance section
          document.Add(new Paragraph("\n\n", normalFont));
          var acceptanceHeading = new Paragraph("Acceptance:", headingFont)
          {
            SpacingAfter = 10
          };
          document.Add(acceptanceHeading);

          var acceptanceParagraph = new Paragraph(
            "I, " + candidateName + ", accept the terms and conditions of this offer letter.\n\n\n",
            normalFont
          )
          {
            SpacingAfter = 10
          };
          document.Add(acceptanceParagraph);

          var signatureLine = new Paragraph(
            "Signature: _________________________    Date: _________________________",
            normalFont
          );
          document.Add(signatureLine);

          document.Close();
        }

        _logger.LogInformation("Offer letter PDF generated successfully: {FilePath}", filePath);
        return $"/uploads/offerletters/{fileName}";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error generating offer letter PDF");
        throw;
      }
    });
  }

  public async Task<byte[]> GetPdfBytesAsync(string filePath)
  {
    try
    {
      var fullPath = Path.Combine(_environment.ContentRootPath, filePath.TrimStart('/'));
      
      if (!File.Exists(fullPath))
      {
        throw new FileNotFoundException("PDF file not found", filePath);
      }

      return await File.ReadAllBytesAsync(fullPath);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error reading PDF file: {FilePath}", filePath);
      throw;
    }
  }
}
