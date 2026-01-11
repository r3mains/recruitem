namespace backend.Services.IServices;

public interface INotificationService
{
  Task NotifyStageChangeAsync(Guid jobApplicationId, string oldStatus, string newStatus);
  Task NotifyInterviewScheduledAsync(Guid interviewId);
  Task NotifyOfferGeneratedAsync(Guid offerLetterId);
  Task NotifyDocumentVerificationAsync(Guid verificationId);
  Task SendReminderAsync(Guid userId, string title, string message, string? relatedEntityType = null, Guid? relatedEntityId = null);
}
