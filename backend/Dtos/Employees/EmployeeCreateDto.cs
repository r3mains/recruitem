namespace Backend.Dtos.Employees;

public class EmployeeCreateDto
{
  public Guid UserId { get; set; }
  public string? FullName { get; set; }
  public Guid? BranchAddressId { get; set; }
  public DateOnly? JoiningDate { get; set; }
  public string? OfferLetterUrl { get; set; }
}
