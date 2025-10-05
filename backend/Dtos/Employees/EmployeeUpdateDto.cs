namespace Backend.Dtos.Employees;

public class EmployeeUpdateDto
{
  public string? FullName { get; set; }
  public Guid? BranchAddressId { get; set; }
  public DateOnly? JoiningDate { get; set; }
  public string? OfferLetterUrl { get; set; }
}
