namespace backend.Models;

public class EmailRequest
{
  public string To { get; set; } = string.Empty;
  public string Subject { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;
  public bool IsHtml { get; set; } = true;
  public string? ToName { get; set; }
}

public static class EmailTemplateConstants
{
  public const string PASSWORD_RESET = "password-reset";
  public const string EMAIL_CONFIRMATION = "email-confirmation";
  public const string INTERVIEW_SCHEDULED = "interview-scheduled";
  public const string OFFER_LETTER = "offer-letter";
  public const string APPLICATION_STATUS = "application-status";
}
