using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentEmail.Core;
using FluentEmail.Smtp;
using backend;
using backend.Data;
using backend.Models;
using backend.Middlewares;
using backend.Conventions;
using backend.Consts;

var builder = WebApplication.CreateBuilder(args);

// Versioning
builder.Services.AddApiVersioning(options =>
{
  options.AssumeDefaultVersionWhenUnspecified = true;
  options.DefaultApiVersion = new ApiVersion(1, 0);
  options.ReportApiVersions = true;
});

// Versioned API Explorer
builder.Services.AddVersionedApiExplorer(options =>
{
  options.GroupNameFormat = "'v'VVV";
  options.SubstituteApiVersionInUrl = true;
});

// Swagger configuration
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// Logging
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Controllers
builder.Services.AddControllers(options =>
{
  options.Conventions.Insert(0, new RoutePrefixConvention(new RouteAttribute("api/v{version:apiVersion}")));
});

// CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
  });
});

// Validation
builder.Services.AddFluentValidationAutoValidation(options => options.DisableDataAnnotationsValidation = true);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Database
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                      ?? throw new InvalidOperationException("Database connection string not found in configuration or environment variables.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// Identity Core
builder.Services.AddIdentityCore<User>(options =>
{
  options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Repositories
builder.Services.AddScoped<backend.Repositories.IRepositories.IAuthRepository, backend.Repositories.AuthRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IUserRepository, backend.Repositories.UserRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IProfileRepository, backend.Repositories.ProfileRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IJobRepository, backend.Repositories.JobRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IEmployeeRepository, backend.Repositories.EmployeeRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IAddressRepository, backend.Repositories.AddressRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IPositionRepository, backend.Repositories.PositionRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.ISkillRepository, backend.Repositories.SkillRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IJobTypeRepository, backend.Repositories.JobTypeRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IQualificationRepository, backend.Repositories.QualificationRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.ICandidateRepository, backend.Repositories.CandidateRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IJobApplicationRepository, backend.Repositories.JobApplicationRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IScreeningRepository, backend.Repositories.ScreeningRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IInterviewRepository, backend.Repositories.InterviewRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IEventRepository, backend.Repositories.EventRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IVerificationRepository, backend.Repositories.VerificationRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IDocumentRepository, backend.Repositories.DocumentRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IReportsRepository, backend.Repositories.ReportsRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IEmailTemplateRepository, backend.Repositories.EmailTemplateRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.IOfferLetterRepository, backend.Repositories.OfferLetterRepository>();
builder.Services.AddScoped<backend.Repositories.IRepositories.INotificationRepository, backend.Repositories.NotificationRepository>();

// Services
builder.Services.AddScoped<backend.Services.IServices.IFileStorageService, backend.Services.LocalFileStorageService>();
builder.Services.AddScoped<backend.Services.IServices.IExportService, backend.Services.ExportService>();
builder.Services.AddScoped<backend.Services.IServices.IResumeParserService, backend.Services.ResumeParserService>();
builder.Services.AddScoped<backend.Services.IServices.IPdfGenerationService, backend.Services.PdfGenerationService>();
builder.Services.AddScoped<backend.Services.IServices.ICandidateBulkImportService, backend.Services.CandidateBulkImportService>();
builder.Services.AddScoped<backend.Services.IServices.IScoringService, backend.Services.ScoringService>();
builder.Services.AddScoped<backend.Services.IServices.INotificationService, backend.Services.NotificationService>();

// FluentEmail
var emailHost = builder.Configuration["Email:Host"] ?? Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
var emailPort = int.Parse(builder.Configuration["Email:Port"] ?? Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
var emailUsername = builder.Configuration["Email:Username"] ?? Environment.GetEnvironmentVariable("EMAIL_USERNAME") ?? throw new InvalidOperationException("Email username not found in configuration or environment variables.");
var emailPassword = builder.Configuration["Email:Password"] ?? Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? throw new InvalidOperationException("Email password not found in configuration or environment variables.");
var fromEmail = builder.Configuration["Email:FromEmail"] ?? Environment.GetEnvironmentVariable("FROM_EMAIL") ?? emailUsername;
var fromName = builder.Configuration["Email:FromName"] ?? Environment.GetEnvironmentVariable("FROM_NAME") ?? "Recruitment System";

builder.Services
    .AddFluentEmail(fromEmail, fromName)
    .AddSmtpSender(emailHost, emailPort, emailUsername, emailPassword);

// Email Service
builder.Services.AddScoped<backend.Services.IServices.IEmailService, backend.Services.FluentEmailService>();

// Authentication
// JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT secret key not found in configuration or environment variables.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "https://localhost:7142";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "https://localhost:7142";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = jwtIssuer,
      ValidAudience = jwtAudience,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
      RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    };
  });

// Authorization with RBAC Policies
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("AdminPolicy", policy => policy.RequireRole(Roles.Admin));
  options.AddPolicy("ManageJobs", policy => policy.RequireRole(Roles.Admin, Roles.Recruiter));
  options.AddPolicy("ViewJobs", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer, Roles.Candidate, Roles.Viewer));
  options.AddPolicy("CloseJobs", policy => policy.RequireRole(Roles.Admin, Roles.Recruiter, Roles.HR));
  options.AddPolicy("ManageCandidates", policy => policy.RequireRole(Roles.Admin, Roles.Recruiter));
  options.AddPolicy("ViewCandidates", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer, Roles.Viewer));
  options.AddPolicy("UploadCandidates", policy => policy.RequireRole(Roles.Admin, Roles.Recruiter));
  options.AddPolicy("ScreenResumes", policy => policy.RequireRole(Roles.Admin, Roles.Reviewer, Roles.Recruiter));
  options.AddPolicy("AddReviewComments", policy => policy.RequireRole(Roles.Admin, Roles.Reviewer, Roles.Recruiter, Roles.Interviewer));
  options.AddPolicy("AssignReviewers", policy => policy.RequireRole(Roles.Admin, Roles.Recruiter));
  options.AddPolicy("ShortlistCandidates", policy => policy.RequireRole(Roles.Admin, Roles.Reviewer, Roles.Recruiter));
  options.AddPolicy("ScheduleInterviews", policy => policy.RequireRole(Roles.Admin, Roles.Recruiter, Roles.HR));
  options.AddPolicy("ViewInterviews", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Viewer));
  options.AddPolicy("ConductInterviews", policy => policy.RequireRole(Roles.Admin, Roles.Interviewer, Roles.HR));
  options.AddPolicy("AddInterviewFeedback", policy => policy.RequireRole(Roles.Admin, Roles.Interviewer, Roles.HR));
  options.AddPolicy("ViewInterviewFeedback", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Viewer));
  options.AddPolicy("AssignInterviewers", policy => policy.RequireRole(Roles.Admin, Roles.Recruiter, Roles.HR));
  options.AddPolicy("ManageEvents", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter));
  options.AddPolicy("VerifyDocuments", policy => policy.RequireRole(Roles.Admin, Roles.HR));
  options.AddPolicy("BackgroundVerification", policy => policy.RequireRole(Roles.Admin, Roles.HR));
  options.AddPolicy("FinalSelection", policy => policy.RequireRole(Roles.Admin, Roles.HR));
  options.AddPolicy("GenerateOfferLetters", policy => policy.RequireRole(Roles.Admin, Roles.HR));
  options.AddPolicy("SetJoiningDate", policy => policy.RequireRole(Roles.Admin, Roles.HR));
  options.AddPolicy("ChangeStatus", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter));
  options.AddPolicy("HoldProfiles", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer));
  options.AddPolicy("ManageUsers", policy => policy.RequireRole(Roles.Admin));
  options.AddPolicy("ManageRoles", policy => policy.RequireRole(Roles.Admin));
  options.AddPolicy("ViewUsers", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Viewer));
  options.AddPolicy("ViewReports", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Viewer));
  options.AddPolicy("GenerateReports", policy => policy.RequireRole(Roles.Admin, Roles.HR));
  options.AddPolicy("ViewDashboard", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer));
  options.AddPolicy("ManageExams", policy => policy.RequireRole(Roles.Admin, Roles.Recruiter, Roles.HR));
  options.AddPolicy("TakeExams", policy => policy.RequireRole(Roles.Candidate));
  options.AddPolicy("ViewExamResults", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer));
  options.AddPolicy("CandidatePortal", policy => policy.RequireRole(Roles.Candidate));
  options.AddPolicy("ManageSettings", policy => policy.RequireRole(Roles.Admin, Roles.HR));
  options.AddPolicy("ViewSettings", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter));
  options.AddPolicy("SendEmails", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter));
  options.AddPolicy("ViewOfferLetters", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Candidate));
  options.AddPolicy("ManageOfferLetters", policy => policy.RequireRole(Roles.Admin, Roles.HR));
  options.AddPolicy("UploadDocuments", policy => policy.RequireRole(Roles.Candidate, Roles.Admin, Roles.HR, Roles.Recruiter));
  options.AddPolicy("ViewDocuments", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer, Roles.Viewer, Roles.Candidate));
  options.AddPolicy("ManageDocuments", policy => policy.RequireRole(Roles.Admin, Roles.HR));
  options.AddPolicy("ViewJobOpenings", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer, Roles.Candidate, Roles.Viewer));
  options.AddPolicy("SystemConfig", policy => policy.RequireRole(Roles.Admin));
  options.AddPolicy("ViewLogs", policy => policy.RequireRole(Roles.Admin));
  options.AddPolicy("ManageNotifications", policy => policy.RequireRole(Roles.Admin, Roles.HR));
  options.AddPolicy("ViewerAccess", policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer, Roles.Viewer));
});

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Development Environment
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
      options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
    }
  });

  using var scope = app.Services.CreateScope();

  var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
  var rolesToCreate = new[]
  {
    Roles.Recruiter,
    Roles.HR,
    Roles.Interviewer,
    Roles.Reviewer,
    Roles.Admin,
    Roles.Candidate,
    Roles.Viewer
  };

  foreach (var roleName in rolesToCreate)
  {
    if (!await roleManager.RoleExistsAsync(roleName))
    {
      await roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
    }
  }
}

// Global Exception Handler
app.UseMiddleware<GlobalExceptionHandler>();

// CORS
app.UseCors("AllowFrontend");

// HTTPS Redirection
app.UseHttpsRedirection();

// Authentication
app.UseAuthentication();

// Authorization
app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();

