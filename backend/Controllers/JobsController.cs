using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recruitem_backend.Data;
using recruitem_backend.Models;
using System.Security.Claims;

namespace recruitem_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<JobsController> _logger;

        public JobsController(DatabaseContext context, ILogger<JobsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs()
        {
            try
            {
                _logger.LogInformation("Getting all jobs from database");

                var jobs = await _context.Jobs
                    .Include(j => j.Recruiter)
                    .ThenInclude(r => r.Role)
                    .ToListAsync();

                var jobList = new List<object>();
                foreach (var job in jobs)
                {
                    var jobData = new
                    {
                        id = job.Id,
                        title = job.Title,
                        description = job.Description,
                        location = job.Location,
                        salaryMin = job.SalaryMin,
                        salaryMax = job.SalaryMax,
                        createdAt = job.CreatedAt,
                        updatedAt = job.UpdatedAt,
                        recruiter = new
                        {
                            id = job.Recruiter.Id,
                            email = job.Recruiter.Email,
                            role = job.Recruiter.Role.RoleName
                        }
                    };
                    jobList.Add(jobData);
                }

                _logger.LogInformation($"Found {jobList.Count} jobs");
                return Ok(jobList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting jobs: {ex.Message}");
                return BadRequest("Could not get jobs. Please try again.");
            }
        }        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(Guid id)
        {
            try
            {
                _logger.LogInformation($"Looking for job with ID: {id}");

                var job = await _context.Jobs
                    .Include(j => j.Recruiter)
                    .ThenInclude(r => r.Role)
                    .FirstOrDefaultAsync(j => j.Id == id);

                if (job == null)
                {
                    _logger.LogWarning($"Job not found with ID: {id}");
                    return NotFound("Job not found");
                }

                var jobResponse = new
                {
                    id = job.Id,
                    title = job.Title,
                    description = job.Description,
                    location = job.Location,
                    salaryMin = job.SalaryMin,
                    salaryMax = job.SalaryMax,
                    createdAt = job.CreatedAt,
                    updatedAt = job.UpdatedAt,
                    recruiter = new
                    {
                        id = job.Recruiter.Id,
                        email = job.Recruiter.Email,
                        role = job.Recruiter.Role.RoleName
                    }
                };

                return Ok(jobResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting job: {ex.Message}");
                return BadRequest("Could not get job details. Please try again.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description) || string.IsNullOrEmpty(request.Location))
                {
                    return BadRequest("Title, description, and location are required");
                }

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized("You must be logged in");
                }

                Guid userId;
                if (!Guid.TryParse(userIdString, out userId))
                {
                    return Unauthorized("Invalid user token");
                }

                _logger.LogInformation($"User {userId} trying to create job: {request.Title}");

                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return Unauthorized("User not found");
                }

                if (user.Role.RoleName != "Recruiter")
                {
                    return BadRequest("Only recruiters can create jobs");
                }

                var newJob = new Job();
                newJob.Id = Guid.NewGuid();
                newJob.RecruiterId = userId;
                newJob.Title = request.Title;
                newJob.Description = request.Description;
                newJob.Location = request.Location;
                newJob.SalaryMin = request.SalaryMin;
                newJob.SalaryMax = request.SalaryMax;
                newJob.CreatedAt = DateTime.UtcNow;
                newJob.UpdatedAt = DateTime.UtcNow;

                _context.Jobs.Add(newJob);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Job created successfully with ID: {newJob.Id}");

                return Ok(new
                {
                    message = "Job created successfully",
                    job = new
                    {
                        id = newJob.Id,
                        title = newJob.Title,
                        description = newJob.Description,
                        location = newJob.Location,
                        salaryMin = newJob.SalaryMin,
                        salaryMax = newJob.SalaryMax,
                        createdAt = newJob.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating job: {ex.Message}");
                return BadRequest("Could not create job. Please try again.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(Guid id, [FromBody] UpdateJobRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Update job request validation failed");
                return BadRequest(ModelState);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                _logger.LogWarning("Update job failed: Invalid user ID");
                return Unauthorized("Invalid user");
            }

            _logger.LogInformation("Updating job: {JobId} by user: {UserId}", id, userId);

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
            if (job == null)
            {
                _logger.LogWarning("Update job failed: Job not found: {JobId}", id);
                return NotFound("Job not found");
            }

            if (job.RecruiterId != userId)
            {
                _logger.LogWarning("Update job failed: User not authorized: {UserId} for job: {JobId}", userId, id);
                return Forbid("You can only update your own jobs");
            }

            job.Title = request.Title;
            job.Description = request.Description;
            job.Location = request.Location;
            job.SalaryMin = request.SalaryMin;
            job.SalaryMax = request.SalaryMax;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Job updated successfully: {JobId}", job.Id);

            return Ok(new
            {
                job.Id,
                job.Title,
                job.Description,
                job.Location,
                job.SalaryMin,
                job.SalaryMax,
                job.CreatedAt,
                job.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(Guid id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                _logger.LogWarning("Delete job failed: Invalid user ID");
                return Unauthorized("Invalid user");
            }

            _logger.LogInformation("Deleting job: {JobId} by user: {UserId}", id, userId);

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
            if (job == null)
            {
                _logger.LogWarning("Delete job failed: Job not found: {JobId}", id);
                return NotFound("Job not found");
            }

            if (job.RecruiterId != userId)
            {
                _logger.LogWarning("Delete job failed: User not authorized: {UserId} for job: {JobId}", userId, id);
                return Forbid("You can only delete your own jobs");
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Job deleted successfully: {JobId}", job.Id);

            return Ok("Job deleted successfully");
        }
    }

    public class CreateJobRequest
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Location { get; set; } = "";
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
    }

    public class UpdateJobRequest
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Location { get; set; } = "";
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
    }
}
