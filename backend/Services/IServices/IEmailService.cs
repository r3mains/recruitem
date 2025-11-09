using backend.Models;

namespace backend.Services.IServices;

public interface IEmailService
{
  Task<bool> SendEmailAsync(EmailRequest request);
  Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string userName);
  Task<bool> SendEmailConfirmationAsync(string email, string confirmationToken, string userName);
  Task<bool> SendInterviewScheduledEmailAsync(string email, string candidateName, DateTime interviewDate, string jobTitle);
  Task<bool> SendOfferLetterAsync(string email, string candidateName, string jobTitle, string offerDetails);
  Task<bool> SendApplicationStatusUpdateAsync(string email, string candidateName, string jobTitle, string status, string? comments = null);
}
