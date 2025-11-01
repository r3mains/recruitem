using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Extensions;

public static class DataSeederExtensions
{
  public static async Task SeedStatusTypesAsync(this AppDbContext context)
  {
    if (await context.StatusTypes.AnyAsync()) return;

    var statusTypes = new List<StatusType>
    {
      new() { Context = "application", Status = "Applied" },
      new() { Context = "application", Status = "Under Review" },
      new() { Context = "application", Status = "Shortlisted" },
      new() { Context = "application", Status = "Accepted" },
      new() { Context = "application", Status = "Rejected" },
      new() { Context = "job", Status = "Open" },
      new() { Context = "job", Status = "Closed" },
      new() { Context = "job", Status = "On Hold" },
      new() { Context = "position", Status = "Open" },
      new() { Context = "position", Status = "Closed" },
      new() { Context = "interview", Status = "Scheduled" },
      new() { Context = "interview", Status = "Completed" },
      new() { Context = "interview", Status = "Cancelled" },
    };

    await context.StatusTypes.AddRangeAsync(statusTypes);
    await context.SaveChangesAsync();
  }
}
