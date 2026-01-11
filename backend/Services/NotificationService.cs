using backend.Models;
using backend.DTOs.Interview;
using backend.Repositories.IRepositories;
using backend.Services.IServices;
using Microsoft.AspNetCore.Identity;
using AppStatus = backend.Consts.ApplicationStatus;
using VerifStatus = backend.Consts.VerificationStatus;

namespace backend.Services;

public class NotificationService(
  INotificationRepository notificationRepository,
  IJobApplicationRepository jobApplicationRepository,
  IInterviewRepository interviewRepository,
  IOfferLetterRepository offerLetterRepository,
  IVerificationRepository verificationRepository,
  IEmailService emailService,
  UserManager<User> userManager) : INotificationService
{
  private readonly INotificationRepository _notificationRepository = notificationRepository;
  private readonly IJobApplicationRepository _jobApplicationRepository = jobApplicationRepository;
  private readonly IInterviewRepository _interviewRepository = interviewRepository;
  private readonly IOfferLetterRepository _offerLetterRepository = offerLetterRepository;
  private readonly IVerificationRepository _verificationRepository = verificationRepository;
  private readonly IEmailService _emailService = emailService;
  private readonly UserManager<User> _userManager = userManager;

  public async Task NotifyStageChangeAsync(Guid jobApplicationId, string oldStatus, string newStatus)
  {
    var application = await _jobApplicationRepository.GetByIdAsync(jobApplicationId);
    if (application == null) return;

    var stageTemplates = GetStageNotificationTemplates();
    var template = stageTemplates.FirstOrDefault(t => t.Stage == newStatus);
    if (template == null) return;

    // Notify candidate
    var candidateNotification = new Notification
    {
      Id = Guid.NewGuid(),
      UserId = application.Candidate.UserId,
      Title = template.NotificationTitle,
      Message = template.NotificationMessage
        .Replace("{{JobTitle}}", application.Job?.Title ?? "the position")
        .Replace("{{Status}}", newStatus),
      Type = "Info",
      RelatedEntityType = "JobApplication",
      RelatedEntityId = jobApplicationId,
      CreatedAt = DateTime.UtcNow
    };
    await _notificationRepository.CreateAsync(candidateNotification);

    // Send email to candidate
    try
    {
      await _emailService.SendEmailAsync(new EmailRequest
      {
        To = application.Candidate.User.Email!,
        ToName = application.Candidate.FullName ?? "Candidate",
        Subject = template.EmailSubject.Replace("{{JobTitle}}", application.Job?.Title ?? "the position"),
        Body = template.EmailBody
          .Replace("{{CandidateName}}", application.Candidate.FullName ?? "Candidate")
          .Replace("{{JobTitle}}", application.Job?.Title ?? "the position")
          .Replace("{{Status}}", newStatus),
        IsHtml = true
      });
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Failed to send email notification: {ex.Message}");
    }

    // Notify relevant roles
    foreach (var role in template.NotifyRoles)
    {
      var usersInRole = await _userManager.GetUsersInRoleAsync(role);
      foreach (var user in usersInRole)
      {
        var roleNotification = new Notification
        {
          Id = Guid.NewGuid(),
          UserId = user.Id,
          Title = $"Application Status Update",
          Message = $"Application for {application.Candidate.FullName} for {application.Job?.Title} moved to {newStatus}",
          Type = "Info",
          RelatedEntityType = "JobApplication",
          RelatedEntityId = jobApplicationId,
          CreatedAt = DateTime.UtcNow
        };
        await _notificationRepository.CreateAsync(roleNotification);
      }
    }
  }

  public async Task NotifyInterviewScheduledAsync(Guid interviewId)
  {
    var interview = await _interviewRepository.GetInterviewByIdAsync(interviewId);
    if (interview == null) return;

    var application = await _jobApplicationRepository.GetByIdAsync(interview.JobApplicationId);
    if (application == null) return;

    var scheduleDate = interview.Schedules?.FirstOrDefault()?.ScheduledAt;
    var scheduleDateStr = scheduleDate.HasValue ? scheduleDate.Value.ToString("MMM dd, yyyy HH:mm") : "TBD";

    // Notify candidate
    var candidateNotification = new Notification
    {
      Id = Guid.NewGuid(),
      UserId = application.Candidate.UserId,
      Title = "Interview Scheduled",
      Message = $"Your interview for {interview.JobTitle} has been scheduled for {scheduleDateStr}",
      Type = "Success",
      RelatedEntityType = "Interview",
      RelatedEntityId = interviewId,
      CreatedAt = DateTime.UtcNow
    };
    await _notificationRepository.CreateAsync(candidateNotification);

    // Notify interviewers
    var interviewers = interview.Interviewers ?? new List<InterviewerDto>();
    foreach (var interviewer in interviewers)
    {
      var interviewerNotification = new Notification
      {
        Id = Guid.NewGuid(),
        UserId = interviewer.InterviewerId,
        Title = "Interview Assigned",
        Message = $"You have been assigned to interview {interview.CandidateName} on {scheduleDateStr}",
        Type = "Info",
        RelatedEntityType = "Interview",
        RelatedEntityId = interviewId,
        CreatedAt = DateTime.UtcNow
      };
      await _notificationRepository.CreateAsync(interviewerNotification);
    }

    // Send emails
    try
    {
      var scheduleFullDateStr = scheduleDate.HasValue ? scheduleDate.Value.ToString("MMMM dd, yyyy HH:mm") : "TBD";
      await _emailService.SendEmailAsync(new EmailRequest
      {
        To = application.Candidate.User.Email!,
        ToName = interview.CandidateName ?? "Candidate",
        Subject = "Interview Scheduled",
        Body = $"Dear {interview.CandidateName},<br><br>" +
          $"Your interview for the position of {interview.JobTitle} has been scheduled.<br><br>" +
          $"<strong>Date & Time:</strong> {scheduleFullDateStr}<br>" +
          $"<strong>Type:</strong> {interview.InterviewType}<br><br>" +
          $"Best regards,<br>HR Team",
        IsHtml = true
      });
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Failed to send interview notification email: {ex.Message}");
    }
  }

  public async Task NotifyOfferGeneratedAsync(Guid offerLetterId)
  {
    var offer = await _offerLetterRepository.GetOfferLetterByIdAsync(offerLetterId);
    if (offer == null) return;

    var application = await _jobApplicationRepository.GetByIdAsync(offer.JobApplicationId);
    if (application == null) return;

    var notification = new Notification
    {
      Id = Guid.NewGuid(),
      UserId = application.Candidate.UserId,
      Title = "Offer Letter Generated",
      Message = $"Congratulations! Your offer letter for {application.Job?.Title} has been generated",
      Type = "Success",
      RelatedEntityType = "OfferLetter",
      RelatedEntityId = offerLetterId,
      CreatedAt = DateTime.UtcNow
    };
    await _notificationRepository.CreateAsync(notification);
  }

  public async Task NotifyDocumentVerificationAsync(Guid verificationId)
  {
    var verification = await _verificationRepository.GetVerificationByIdAsync(verificationId);
    if (verification == null) return;

    // Get all applications for this candidate to find UserId
    var applications = await _jobApplicationRepository.GetAllAsync(null, 1, 1000, null, verification.CandidateId, null);
    var firstApp = applications?.FirstOrDefault();
    
    if (firstApp?.Candidate?.UserId == null) return;

    var notification = new Notification
    {
      Id = Guid.NewGuid(),
      UserId = firstApp.Candidate.UserId,
      Title = "Document Verification Update",
      Message = $"Your document verification status has been updated to: {verification.Status}",
      Type = verification.Status == VerifStatus.Approved ? "Success" : "Info",
      RelatedEntityType = "Verification",
      RelatedEntityId = verificationId,
      CreatedAt = DateTime.UtcNow
    };
    await _notificationRepository.CreateAsync(notification);
  }

  public async Task SendReminderAsync(Guid userId, string title, string message, string? relatedEntityType = null, Guid? relatedEntityId = null)
  {
    var notification = new Notification
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Title = title,
      Message = message,
      Type = "Warning",
      RelatedEntityType = relatedEntityType,
      RelatedEntityId = relatedEntityId,
      CreatedAt = DateTime.UtcNow
    };
    await _notificationRepository.CreateAsync(notification);
  }

  private List<StageNotificationTemplate> GetStageNotificationTemplates()
  {
    return new List<StageNotificationTemplate>
    {
      new()
      {
        Stage = AppStatus.Shortlisted,
        EmailSubject = "Application Shortlisted - {{JobTitle}}",
        EmailBody = "Dear {{CandidateName}},<br><br>Congratulations! Your application for {{JobTitle}} has been shortlisted. We will contact you soon for the next steps.<br><br>Best regards,<br>HR Team",
        NotificationTitle = "Application Shortlisted",
        NotificationMessage = "Your application for {{JobTitle}} has been shortlisted!",
        NotifyRoles = new List<string> { backend.Consts.Roles.Recruiter, backend.Consts.Roles.HR }
      },
      new()
      {
        Stage = AppStatus.Interviewing,
        EmailSubject = "Interview Stage - {{JobTitle}}",
        EmailBody = "Dear {{CandidateName}},<br><br>Your application for {{JobTitle}} has moved to the interview stage. You will receive interview details shortly.<br><br>Best regards,<br>HR Team",
        NotificationTitle = "Moved to Interview Stage",
        NotificationMessage = "Your application for {{JobTitle}} is now in the interview stage",
        NotifyRoles = new List<string> { backend.Consts.Roles.Recruiter, backend.Consts.Roles.HR, backend.Consts.Roles.Interviewer }
      },
      new()
      {
        Stage = AppStatus.Offered,
        EmailSubject = "Job Offer - {{JobTitle}}",
        EmailBody = "Dear {{CandidateName}},<br><br>Congratulations! We are pleased to offer you the position of {{JobTitle}}. Please check your offer letter for details.<br><br>Best regards,<br>HR Team",
        NotificationTitle = "Job Offer",
        NotificationMessage = "Congratulations! You have received a job offer for {{JobTitle}}",
        NotifyRoles = new List<string> { backend.Consts.Roles.HR, backend.Consts.Roles.Recruiter }
      },
      new()
      {
        Stage = AppStatus.Rejected,
        EmailSubject = "Application Update - {{JobTitle}}",
        EmailBody = "Dear {{CandidateName}},<br><br>Thank you for your interest in {{JobTitle}}. After careful consideration, we have decided to move forward with other candidates. We wish you the best in your job search.<br><br>Best regards,<br>HR Team",
        NotificationTitle = "Application Update",
        NotificationMessage = "Your application for {{JobTitle}} status has been updated",
        NotifyRoles = new List<string> { backend.Consts.Roles.Recruiter, backend.Consts.Roles.HR }
      },
      new()
      {
        Stage = AppStatus.OnHold,
        EmailSubject = "Application On Hold - {{JobTitle}}",
        EmailBody = "Dear {{CandidateName}},<br><br>Your application for {{JobTitle}} has been placed on hold. We will notify you of any updates.<br><br>Best regards,<br>HR Team",
        NotificationTitle = "Application On Hold",
        NotificationMessage = "Your application for {{JobTitle}} has been placed on hold",
        NotifyRoles = new List<string> { backend.Consts.Roles.Recruiter, backend.Consts.Roles.HR }
      }
    };
  }
}
