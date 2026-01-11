using backend.DTOs.Resume;
using backend.Services.IServices;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;

namespace backend.Services;

public class ResumeParserService(ILogger<ResumeParserService> logger) : IResumeParserService
{
  private readonly ILogger<ResumeParserService> _logger = logger;

  public async Task<ParsedResumeDto> ParseResumeAsync(IFormFile file)
  {
    try
    {
      string text;
      
      using (var stream = file.OpenReadStream())
      {
        if (file.ContentType == "application/pdf" || file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
          text = await ExtractTextFromPdfAsync(stream);
        }
        else
        {
          // For text files or other formats
          using var reader = new StreamReader(stream);
          text = await reader.ReadToEndAsync();
        }
      }

      return ExtractInformationFromText(text);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error parsing resume: {FileName}", file.FileName);
      throw;
    }
  }

  public async Task<string> ExtractTextFromPdfAsync(Stream pdfStream)
  {
    return await Task.Run(() =>
    {
      try
      {
        using var reader = new PdfReader(pdfStream);
        var text = new System.Text.StringBuilder();

        for (int page = 1; page <= reader.NumberOfPages; page++)
        {
          var pageBytes = reader.GetPageContent(page);
          if (pageBytes != null)
          {
            var pageText = System.Text.Encoding.UTF8.GetString(pageBytes);
            // Basic cleanup of PDF binary markers
            pageText = Regex.Replace(pageText, @"[^\x20-\x7E\r\n]", " ");
            text.Append(pageText);
          }
        }

        return text.ToString();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error extracting text from PDF");
        throw;
      }
    });
  }

  public ParsedResumeDto ExtractInformationFromText(string text)
  {
    var fullName = ExtractName(text);
    var email = ExtractEmail(text);
    var phone = ExtractPhone(text);
    var skills = ExtractSkills(text);
    var education = ExtractEducation(text);
    var experience = ExtractYearsOfExperience(text);
    var summary = ExtractSummary(text);

    return new ParsedResumeDto(
      fullName,
      email,
      phone,
      text,
      skills,
      education,
      experience,
      summary
    );
  }

  private string? ExtractName(string text)
  {
    // Look for name at the beginning of the resume (first few lines)
    var lines = text.Split('\n').Take(5);
    foreach (var line in lines)
    {
      var trimmed = line.Trim();
      // Name is usually short (2-4 words) and at the top
      if (trimmed.Length > 3 && trimmed.Length < 50 && 
          Regex.IsMatch(trimmed, @"^[A-Z][a-zA-Z\s\.]+$") &&
          !trimmed.Contains("Resume") && !trimmed.Contains("CV"))
      {
        return trimmed;
      }
    }
    return null;
  }

  private string? ExtractEmail(string text)
  {
    var emailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
    var match = Regex.Match(text, emailPattern);
    return match.Success ? match.Value : null;
  }

  private string? ExtractPhone(string text)
  {
    var phonePatterns = new[]
    {
      @"\+?\d{1,3}[-.\s]?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}",
      @"\b\d{10}\b",
      @"\+\d{1,3}\s?\d{10}"
    };

    foreach (var pattern in phonePatterns)
    {
      var match = Regex.Match(text, pattern);
      if (match.Success)
        return match.Value;
    }
    return null;
  }

  private List<string> ExtractSkills(string text)
  {
    var skills = new List<string>();
    var commonSkills = new[]
    {
      // Programming Languages
      "C#", "Java", "Python", "JavaScript", "TypeScript", "C++", "Ruby", "PHP", "Swift", "Kotlin",
      "Go", "Rust", "Scala", "R", "MATLAB", "Perl",
      
      // Web Technologies
      "HTML", "CSS", "React", "Angular", "Vue", "Node.js", "Express", "ASP.NET", "Django", "Flask",
      "Spring Boot", "Laravel", "Rails", "jQuery", "Bootstrap", "Tailwind",
      
      // Databases
      "SQL", "MySQL", "PostgreSQL", "MongoDB", "Redis", "Oracle", "SQL Server", "Cassandra",
      "DynamoDB", "Firebase", "SQLite",
      
      // Cloud & DevOps
      "AWS", "Azure", "GCP", "Docker", "Kubernetes", "Jenkins", "GitLab CI", "GitHub Actions",
      "Terraform", "Ansible", "CI/CD", "DevOps",
      
      // Other Technologies
      "Git", "REST API", "GraphQL", "Microservices", "Machine Learning", "AI", "Data Science",
      "Big Data", "Hadoop", "Spark", "Kafka", "RabbitMQ", "Elasticsearch", "Linux", "Agile",
      "Scrum", "JIRA", "Selenium", "JUnit", "Jest", "Mocha"
    };

    foreach (var skill in commonSkills)
    {
      if (Regex.IsMatch(text, $@"\b{Regex.Escape(skill)}\b", RegexOptions.IgnoreCase))
      {
        if (!skills.Contains(skill, StringComparer.OrdinalIgnoreCase))
        {
          skills.Add(skill);
        }
      }
    }

    return skills;
  }

  private List<string> ExtractEducation(string text)
  {
    var education = new List<string>();
    var degrees = new[] { "Bachelor", "Master", "PhD", "B.Tech", "M.Tech", "B.E.", "M.E.", "MBA", 
                          "B.Sc", "M.Sc", "B.A.", "M.A.", "Diploma" };

    foreach (var degree in degrees)
    {
      var pattern = $@"{Regex.Escape(degree)}[^\n]*";
      var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
      
      foreach (Match match in matches)
      {
        if (!education.Contains(match.Value.Trim()))
        {
          education.Add(match.Value.Trim());
        }
      }
    }

    return education;
  }

  private int? ExtractYearsOfExperience(string text)
  {
    var patterns = new[]
    {
      @"(\d+)\+?\s*years?\s+of\s+experience",
      @"experience[:\s]+(\d+)\+?\s*years?",
      @"(\d+)\+?\s*years?\s+experience"
    };

    foreach (var pattern in patterns)
    {
      var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
      if (match.Success && int.TryParse(match.Groups[1].Value, out int years))
      {
        return years;
      }
    }

    return null;
  }

  private string? ExtractSummary(string text)
  {
    var summaryKeywords = new[] { "summary", "profile", "objective", "about" };
    var lines = text.Split('\n');

    for (int i = 0; i < lines.Length; i++)
    {
      var line = lines[i].Trim().ToLower();
      
      foreach (var keyword in summaryKeywords)
      {
        if (line.Contains(keyword) && line.Length < 50)
        {
          // Take next few lines as summary
          var summaryLines = new List<string>();
          for (int j = i + 1; j < Math.Min(i + 6, lines.Length); j++)
          {
            var summaryLine = lines[j].Trim();
            if (string.IsNullOrWhiteSpace(summaryLine) || 
                summaryLine.ToLower().Contains("experience") ||
                summaryLine.ToLower().Contains("education"))
              break;
            
            summaryLines.Add(summaryLine);
          }
          
          if (summaryLines.Any())
          {
            return string.Join(" ", summaryLines);
          }
        }
      }
    }

    return null;
  }
}
