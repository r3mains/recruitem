using FluentEmail.Core;
using backend.Services.IServices;
using backend.Models;
using backend.Data;

namespace backend.Services;

public class FluentEmailService(IFluentEmail fluentEmail, IConfiguration configuration, ILogger<FluentEmailService> logger, ApplicationDbContext context) : IEmailService
{
  private readonly IFluentEmail _fluentEmail = fluentEmail;
  private readonly IConfiguration _configuration = configuration;
  private readonly ILogger<FluentEmailService> _logger = logger;
  private readonly ApplicationDbContext _context = context;

  public async Task<bool> SendEmailAsync(EmailRequest request)
  {
    var emailLog = new EmailLog
    {
      Id = Guid.NewGuid(),
      ToEmail = request.To,
      Subject = request.Subject,
      Body = request.Body,
      SentAt = DateTime.UtcNow,
      Status = "Pending"
    };

    try
    {
      var email = _fluentEmail
          .To(request.To, request.ToName)
          .Subject(request.Subject);

      if (request.IsHtml)
        email.Body(request.Body, true);
      else
        email.Body(request.Body);

      var result = await email.SendAsync();

      if (result.Successful)
      {
        emailLog.Status = "Sent";
        _logger.LogInformation("Email sent successfully to {Email} via Gmail SMTP", request.To);
        await _context.EmailLogs.AddAsync(emailLog);
        await _context.SaveChangesAsync();
        return true;
      }

      emailLog.Status = "Failed";
      emailLog.ErrorMessage = string.Join(", ", result.ErrorMessages);
      _logger.LogError("Gmail SMTP sending failed: {Errors}", string.Join(", ", result.ErrorMessages));
      await _context.EmailLogs.AddAsync(emailLog);
      await _context.SaveChangesAsync();
      return false;
    }
    catch (Exception ex)
    {
      emailLog.Status = "Failed";
      emailLog.ErrorMessage = ex.Message;
      _logger.LogError(ex, "Failed to send email via Gmail SMTP to {Email}", request.To);
      await _context.EmailLogs.AddAsync(emailLog);
      await _context.SaveChangesAsync();
      return false;
    }
  }

  public async Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string userName)
  {
    try
    {
      var resetUrl = $"{_configuration["Frontend:BaseUrl"]}/reset-password?token={resetToken}&email={email}";
      var template = GetPasswordResetTemplate(userName, resetUrl);

      var result = await _fluentEmail
          .To(email, userName)
          .Subject("Password Reset Request - Recruitment System")
          .Body(template, false)
          .SendAsync();

      if (result.Successful)
      {
        _logger.LogInformation("Password reset email sent successfully via Gmail to {Email}", email);
        return true;
      }

      _logger.LogError("Failed to send password reset email via Gmail: {Errors}", string.Join(", ", result.ErrorMessages));
      return false;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception sending password reset email via Gmail to {Email}", email);
      return false;
    }
  }

  public async Task<bool> SendEmailConfirmationAsync(string email, string confirmationToken, string userName)
  {
    try
    {
      var confirmUrl = $"{_configuration["Frontend:BaseUrl"]}/confirm-email?token={confirmationToken}&email={email}";
      var template = GetEmailConfirmationTemplate(userName, confirmUrl);

      var result = await _fluentEmail
          .To(email, userName)
          .Subject("Confirm Your Email - Recruitment System")
          .Body(template, false)
          .SendAsync();

      return result.Successful;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send email confirmation via Gmail to {Email}", email);
      return false;
    }
  }

  public async Task<bool> SendInterviewScheduledEmailAsync(string email, string candidateName, DateTime interviewDate, string jobTitle)
  {
    try
    {
      var template = GetInterviewScheduledTemplate(candidateName, jobTitle, interviewDate);

      var result = await _fluentEmail
          .To(email, candidateName)
          .Subject($"Interview Scheduled - {jobTitle}")
          .Body(template, false)
          .SendAsync();

      return result.Successful;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send interview notification via Gmail to {Email}", email);
      return false;
    }
  }

  public async Task<bool> SendOfferLetterAsync(string email, string candidateName, string jobTitle, string offerDetails)
  {
    try
    {
      var template = GetOfferLetterTemplate(candidateName, jobTitle, offerDetails);

      var result = await _fluentEmail
          .To(email, candidateName)
          .Subject($"Job Offer - {jobTitle}")
          .Body(template, false)
          .SendAsync();

      return result.Successful;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send offer letter via Gmail to {Email}", email);
      return false;
    }
  }

  public async Task<bool> SendApplicationStatusUpdateAsync(string email, string candidateName, string jobTitle, string status, string? comments = null)
  {
    try
    {
      var template = GetApplicationStatusTemplate(candidateName, jobTitle, status, comments);

      var result = await _fluentEmail
          .To(email, candidateName)
          .Subject($"Application Update - {jobTitle}")
          .Body(template, false)
          .SendAsync();

      return result.Successful;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send status update via Gmail to {Email}", email);
      return false;
    }
  }

  #region Email Templates

  private static string GetPasswordResetTemplate(string userName, string resetUrl) =>
    $@"Hello {userName},

You requested a password reset for your account.

Click the link below to reset your password:
{resetUrl}

This link will expire in 24 hours.";

  private static string GetEmailConfirmationTemplate(string userName, string confirmUrl) =>
    $@"Hello {userName},

Please confirm your email address by clicking the link below:

{confirmUrl}";

  private static string GetInterviewScheduledTemplate(string candidateName, string jobTitle, DateTime interviewDate) =>
    $@"Dear {candidateName},

Your interview has been scheduled.

Position: {jobTitle}
Date: {interviewDate:dddd, MMMM dd, yyyy}
Time: {interviewDate:h:mm tt}";

  private static string GetOfferLetterTemplate(string candidateName, string jobTitle, string offerDetails) =>
    $@"Dear {candidateName},

We are pleased to offer you the position: {jobTitle}

{offerDetails}";

  private static string GetApplicationStatusTemplate(string candidateName, string jobTitle, string status, string? comments)
  {
    var commentsSection = string.IsNullOrEmpty(comments) ? "" : $@"

Comments: {comments}";

    return $@"Dear {candidateName},

Application Status Update for: {jobTitle}

Status: {status.ToUpper()}{commentsSection}";
  }

  public async Task<bool> SendEmailWithTemplateAsync(Guid templateId, string toEmail, string? toName, Dictionary<string, string> variables)
  {
    try
    {
      var template = await _context.EmailTemplates.FindAsync(templateId);
      if (template == null)
      {
        _logger.LogError("Email template not found: {TemplateId}", templateId);
        return false;
      }

      if (!template.IsActive)
      {
        _logger.LogWarning("Attempted to use inactive template: {TemplateId}", templateId);
        return false;
      }

      var subject = ApplyVariablesToTemplate(template.Subject, variables).Result;
      var body = ApplyVariablesToTemplate(template.Body, variables).Result;

      var emailRequest = new EmailRequest
      {
        To = toEmail,
        ToName = toName ?? toEmail,
        Subject = subject,
        Body = body,
        IsHtml = true
      };

      return await SendEmailAsync(emailRequest);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send email with template {TemplateId} to {Email}", templateId, toEmail);
      return false;
    }
  }

  public Task<string> ApplyVariablesToTemplate(string template, Dictionary<string, string> variables)
  {
    var result = template;
    foreach (var variable in variables)
    {
      result = result.Replace($"{{{{{variable.Key}}}}}", variable.Value);
    }
    return Task.FromResult(result);
  }

  #endregion
}
