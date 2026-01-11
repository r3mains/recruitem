using backend.DTOs.Candidate;
using backend.Models;
using backend.Repositories.IRepositories;
using backend.Services.IServices;
using OfficeOpenXml;
using Microsoft.AspNetCore.Identity;

namespace backend.Services
{
  public class CandidateBulkImportService : ICandidateBulkImportService
  {
    private readonly ICandidateRepository _candidateRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly IQualificationRepository _qualificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public CandidateBulkImportService(
      ICandidateRepository candidateRepository,
      ISkillRepository skillRepository,
      IQualificationRepository qualificationRepository,
      IUserRepository userRepository,
      IPasswordHasher<User> passwordHasher)
    {
      _candidateRepository = candidateRepository;
      _skillRepository = skillRepository;
      _qualificationRepository = qualificationRepository;
      _userRepository = userRepository;
      _passwordHasher = passwordHasher;
    }

    public async Task<BulkImportResultDto> ImportCandidatesFromExcelAsync(Stream excelStream, string fileName)
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var result = new BulkImportResultDto();
      var errors = new List<string>();

      try
      {
        // Load all skills and qualifications once for lookup
        var allSkills = await _skillRepository.GetSkillsAsync(1, 10000);
        var allQualifications = await _qualificationRepository.GetAllAsync(null, 1, 10000);

        var skillDict = allSkills.ToDictionary(
          s => s.SkillName.ToLower(),
          s => s.Id,
          StringComparer.OrdinalIgnoreCase
        );

        var qualificationDict = allQualifications.ToDictionary(
          q => q.QualificationName.ToLower(),
          q => q.Id,
          StringComparer.OrdinalIgnoreCase
        );

        using var package = new ExcelPackage(excelStream);
        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Dimension?.Rows ?? 0;

        if (rowCount < 2)
        {
          result.Errors.Add("Excel file is empty or has no data rows");
          return result;
        }

        for (int row = 2; row <= rowCount; row++)
        {
          try
          {
            var fullName = worksheet.Cells[row, 1].Text?.Trim();
            var email = worksheet.Cells[row, 2].Text?.Trim();
            var contactNumber = worksheet.Cells[row, 3].Text?.Trim();
            var skillsText = worksheet.Cells[row, 4].Text?.Trim();
            var qualificationsText = worksheet.Cells[row, 5].Text?.Trim();
            var city = worksheet.Cells[row, 6].Text?.Trim();
            var state = worksheet.Cells[row, 7].Text?.Trim();
            var country = worksheet.Cells[row, 8].Text?.Trim();

            // Validation
            if (string.IsNullOrWhiteSpace(fullName))
            {
              result.FailedRecords++;
              result.Errors.Add($"Row {row}: Full name is required");
              continue;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
              result.FailedRecords++;
              result.Errors.Add($"Row {row}: Email is required");
              continue;
            }

            // Check if user already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
              result.FailedRecords++;
              result.Errors.Add($"Row {row}: User with email {email} already exists");
              continue;
            }

            // Create user
            var user = new User
            {
              Id = Guid.NewGuid(),
              UserName = email,
              Email = email,
              NormalizedEmail = email.ToUpper(),
              NormalizedUserName = email.ToUpper(),
              EmailConfirmed = false,
              CreatedAt = DateTime.UtcNow
            };
            
            // Set password hash
            user.PasswordHash = _passwordHasher.HashPassword(user, "DefaultPassword123!");

            // Create candidate
            var candidate = new Candidate
            {
              Id = Guid.NewGuid(),
              UserId = user.Id,
              User = user,
              FullName = fullName,
              ContactNumber = contactNumber,
              CreatedAt = DateTime.UtcNow,
              UpdatedAt = DateTime.UtcNow,
              CandidateSkills = new List<CandidateSkill>(),
              CandidateQualifications = new List<CandidateQualification>()
            };

            // Create Address if location data provided
            if (!string.IsNullOrWhiteSpace(city) || !string.IsNullOrWhiteSpace(state) || !string.IsNullOrWhiteSpace(country))
            {
              candidate.Address = new Address
              {
                Id = Guid.NewGuid(),
                Locality = $"{city}, {state}, {country}".Trim(new[] { ',', ' ' })
              };
            }

            // Parse and add skills
            if (!string.IsNullOrWhiteSpace(skillsText))
            {
              var skillNames = skillsText.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
              foreach (var skillName in skillNames)
              {
                var trimmedSkill = skillName.Trim();
                if (skillDict.TryGetValue(trimmedSkill.ToLower(), out var skillId))
                {
                  candidate.CandidateSkills.Add(new CandidateSkill
                  {
                    Id = Guid.NewGuid(),
                    CandidateId = candidate.Id,
                    SkillId = skillId
                  });
                }
                else
                {
                  result.Warnings.Add($"Row {row}: Skill '{trimmedSkill}' not found in system");
                }
              }
            }

            // Parse and add qualifications
            if (!string.IsNullOrWhiteSpace(qualificationsText))
            {
              var qualificationNames = qualificationsText.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
              foreach (var qualName in qualificationNames)
              {
                var trimmedQual = qualName.Trim();
                if (qualificationDict.TryGetValue(trimmedQual.ToLower(), out var qualId))
                {
                  candidate.CandidateQualifications.Add(new CandidateQualification
                  {
                    Id = Guid.NewGuid(),
                    CandidateId = candidate.Id,
                    QualificationId = qualId
                  });
                }
                else
                {
                  result.Warnings.Add($"Row {row}: Qualification '{trimmedQual}' not found in system");
                }
              }
            }

            // Save to database
            await _candidateRepository.CreateCandidateAsync(candidate);
            result.SuccessfulRecords++;
            result.SuccessCount++;
            result.CreatedCandidateIds.Add(candidate.Id);
          }
          catch (Exception ex)
          {
            result.FailedRecords++;
            result.FailureCount++;
            result.Errors.Add($"Row {row}: {ex.Message}");
          }
        }

        result.TotalRecords = rowCount - 1; // Exclude header
        result.TotalRows = rowCount - 1;
      }
      catch (Exception ex)
      {
        result.Errors.Add($"Error processing Excel file: {ex.Message}");
      }

      return result;
    }
  }
}
