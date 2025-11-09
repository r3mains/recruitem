using FluentEmail.Core;
using backend.Services.IServices;
using backend.Models;

namespace backend.Services;

public class FluentEmailService(IFluentEmail fluentEmail, IConfiguration configuration, ILogger<FluentEmailService> logger) : IEmailService
{
  private readonly IFluentEmail _fluentEmail = fluentEmail;
  private readonly IConfiguration _configuration = configuration;
  private readonly ILogger<FluentEmailService> _logger = logger;

  public async Task<bool> SendEmailAsync(EmailRequest request)
  {
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
        _logger.LogInformation("Email sent successfully to {Email} via Gmail SMTP", request.To);
        return true;
      }

      _logger.LogError("Gmail SMTP sending failed: {Errors}", string.Join(", ", result.ErrorMessages));
      return false;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send email via Gmail SMTP to {Email}", request.To);
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

  #endregion
}
