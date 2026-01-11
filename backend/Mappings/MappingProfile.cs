using AutoMapper;
using backend.DTOs;
using backend.DTOs.Document;
using backend.DTOs.Event;
using backend.DTOs.Interview;
using backend.DTOs.Scoring;
using backend.DTOs.Verification;
using backend.Models;

namespace backend.Mappings;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<RegisterDto, User>()
      .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
      .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
      .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

    CreateMap<User, AuthResponseDto>()
      .ConstructUsing(src => new AuthResponseDto(src.Id, src.Email, src.UserName, string.Empty));

    // User mappings
    CreateMap<CreateUserDto, User>()
      .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName ?? src.Email))
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
      .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
      .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

    CreateMap<User, UserDto>();
    CreateMap<User, UserListDto>();

    // Profile mappings
    CreateMap<Employee, backend.DTOs.Profile.EmployeeDto>()
      .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
      .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

    CreateMap<Employee, backend.DTOs.Profile.EmployeeWithDetailsDto>()
      .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
      .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
      .ForMember(dest => dest.BranchAddress, opt => opt.MapFrom(src => src.BranchAddress));

    CreateMap<Candidate, backend.DTOs.Profile.CandidateDto>()
      .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
      .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

    CreateMap<Candidate, backend.DTOs.Profile.CandidateWithDetailsDto>()
      .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
      .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
      .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

    // Address mappings
    CreateMap<Address, backend.DTOs.Location.AddressDto>()
      .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City!.CityName))
      .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.City!.State!.StateName))
      .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.City!.State!.Country!.CountryName));

    CreateMap<Address, backend.DTOs.Location.FullAddressDto>()
      .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City));

    // Location mappings
    CreateMap<Country, backend.DTOs.Location.CountryDto>();
    CreateMap<State, backend.DTOs.Location.StateDto>()
      .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.CountryName));
    CreateMap<City, backend.DTOs.Location.CityDto>()
      .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State.StateName))
      .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.State.Country.CountryName));

    // Job mappings
    CreateMap<backend.DTOs.Job.CreateJobDto, Job>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
      .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
      .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
      .ForMember(dest => dest.JobSkills, opt => opt.Ignore())
      .ForMember(dest => dest.JobQualifications, opt => opt.Ignore());

    CreateMap<Job, backend.DTOs.Job.JobResponseDto>()
      .ForMember(dest => dest.Recruiter, opt => opt.MapFrom(src => src.Recruiter))
      .ForMember(dest => dest.JobType, opt => opt.MapFrom(src => src.JobType))
      .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
      .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
      .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.JobSkills))
      .ForMember(dest => dest.Qualifications, opt => opt.MapFrom(src => src.JobQualifications))
      .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.JobApplications.Count(ja => !ja.IsDeleted)));

    CreateMap<Employee, backend.DTOs.Job.JobRecruiterDto>()
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

    CreateMap<Models.JobType, backend.DTOs.Job.JobTypeDto>();

    CreateMap<Address, backend.DTOs.Job.JobAddressDto>()
      .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City!.CityName))
      .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.City!.State!.StateName))
      .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.City!.State!.Country!.CountryName));

    CreateMap<Position, backend.DTOs.Job.JobPositionDto>()
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Status));

    CreateMap<Models.JobStatus, backend.DTOs.Job.JobStatusDto>();

    CreateMap<JobSkill, backend.DTOs.Job.JobSkillDto>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Skill.Id))
      .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.SkillName));

    CreateMap<JobQualification, backend.DTOs.Job.JobQualificationResponseDto>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Qualification.Id))
      .ForMember(dest => dest.QualificationName, opt => opt.MapFrom(src => src.Qualification.QualificationName));

    // Position mappings
    CreateMap<backend.DTOs.Position.CreatePositionDto, Position>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
      .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
      .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
      .ForMember(dest => dest.PositionSkills, opt => opt.Ignore());

    CreateMap<Position, backend.DTOs.Position.PositionResponseDto>()
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
      .ForMember(dest => dest.Reviewer, opt => opt.MapFrom(src => src.Reviewer))
      .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.PositionSkills))
      .ForMember(dest => dest.JobCount, opt => opt.MapFrom(src => src.Jobs.Count(j => !j.IsDeleted)));

    CreateMap<Models.PositionStatus, backend.DTOs.Position.PositionStatusDto>();

    CreateMap<Employee, backend.DTOs.Position.PositionReviewerDto>()
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

    CreateMap<PositionSkill, backend.DTOs.Position.PositionSkillResponseDto>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Skill.Id))
      .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.SkillName));

    // Skill mappings
    CreateMap<backend.DTOs.Skill.CreateSkillDto, Skill>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

    CreateMap<Skill, backend.DTOs.Skill.SkillResponseDto>()
      .ForMember(dest => dest.PositionCount, opt => opt.MapFrom(src => src.PositionSkills.Count))
      .ForMember(dest => dest.JobCount, opt => opt.MapFrom(src => src.JobSkills.Count))
      .ForMember(dest => dest.CandidateCount, opt => opt.MapFrom(src => src.CandidateSkills.Count));

    // JobType mappings
    CreateMap<backend.DTOs.JobType.CreateJobTypeDto, JobType>();

    CreateMap<backend.DTOs.JobType.UpdateJobTypeDto, JobType>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.Jobs, opt => opt.Ignore());

    CreateMap<JobType, backend.DTOs.JobType.JobTypeResponseDto>()
      .ForMember(dest => dest.JobCount, opt => opt.MapFrom(src => src.Jobs.Count));

    CreateMap<JobType, backend.DTOs.JobType.JobTypeListDto>()
      .ForMember(dest => dest.JobCount, opt => opt.MapFrom(src => src.Jobs.Count));

    // Qualification mappings
    CreateMap<backend.DTOs.Qualification.CreateQualificationDto, Qualification>();

    CreateMap<backend.DTOs.Qualification.UpdateQualificationDto, Qualification>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.JobQualifications, opt => opt.Ignore())
      .ForMember(dest => dest.CandidateQualifications, opt => opt.Ignore());

    CreateMap<Qualification, backend.DTOs.Qualification.QualificationResponseDto>()
      .ForMember(dest => dest.JobCount, opt => opt.MapFrom(src => src.JobQualifications.Count))
      .ForMember(dest => dest.CandidateCount, opt => opt.MapFrom(src => src.CandidateQualifications.Count));

    CreateMap<Qualification, backend.DTOs.Qualification.QualificationListDto>()
      .ForMember(dest => dest.UsageCount, opt => opt.MapFrom(src => src.JobQualifications.Count + src.CandidateQualifications.Count));

    // Candidate mappings
    CreateMap<backend.DTOs.Candidate.CreateCandidateDto, Candidate>();

    CreateMap<backend.DTOs.Candidate.UpdateCandidateDto, Candidate>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.UserId, opt => opt.Ignore())
      .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
      .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
      .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
      .ForMember(dest => dest.User, opt => opt.Ignore())
      .ForMember(dest => dest.Address, opt => opt.Ignore())
      .ForMember(dest => dest.Documents, opt => opt.Ignore())
      .ForMember(dest => dest.JobApplications, opt => opt.Ignore())
      .ForMember(dest => dest.Verifications, opt => opt.Ignore())
      .ForMember(dest => dest.CandidateSkills, opt => opt.Ignore())
      .ForMember(dest => dest.CandidateQualifications, opt => opt.Ignore());

    CreateMap<Candidate, backend.DTOs.Candidate.CandidateResponseDto>()
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
      .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
      .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.CandidateSkills))
      .ForMember(dest => dest.Qualifications, opt => opt.MapFrom(src => src.CandidateQualifications))
      .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.JobApplications.Count))
      .ForMember(dest => dest.DocumentCount, opt => opt.MapFrom(src => src.Documents.Count));

    CreateMap<Candidate, backend.DTOs.Candidate.CandidateListDto>()
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
      .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Address != null && src.Address.City != null && src.Address.City.State != null ? $"{src.Address.City.CityName}, {src.Address.City.State.StateName}" : null))
      .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.JobApplications.Count))
      .ForMember(dest => dest.SkillCount, opt => opt.MapFrom(src => src.CandidateSkills.Count));

    // Candidate Skills mappings
    CreateMap<backend.DTOs.Candidate.CreateCandidateSkillDto, CandidateSkill>();

    CreateMap<CandidateSkill, backend.DTOs.Candidate.CandidateSkillDto>()
      .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.SkillName));

    // Candidate Qualifications mappings
    CreateMap<backend.DTOs.Candidate.CreateCandidateQualificationDto, CandidateQualification>();

    CreateMap<CandidateQualification, backend.DTOs.Candidate.CandidateQualificationDto>()
      .ForMember(dest => dest.QualificationName, opt => opt.MapFrom(src => src.Qualification.QualificationName));

    // Address mappings
    CreateMap<backend.DTOs.Candidate.CreateAddressDto, Address>();

    CreateMap<Address, backend.DTOs.Candidate.AddressDto>()
      .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.CityName : string.Empty))
      .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.City != null && src.City.State != null ? src.City.State.StateName : string.Empty));

    // Job Application mappings
    CreateMap<backend.DTOs.JobApplication.CreateJobApplicationDto, JobApplication>();

    CreateMap<backend.DTOs.JobApplication.UpdateJobApplicationDto, JobApplication>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.JobId, opt => opt.Ignore())
      .ForMember(dest => dest.CandidateId, opt => opt.Ignore())
      .ForMember(dest => dest.AppliedAt, opt => opt.Ignore())
      .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
      .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
      .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
      .ForMember(dest => dest.Job, opt => opt.Ignore())
      .ForMember(dest => dest.Candidate, opt => opt.Ignore())
      .ForMember(dest => dest.Status, opt => opt.Ignore())
      .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
      .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore())
      .ForMember(dest => dest.ApplicationDocuments, opt => opt.Ignore())
      .ForMember(dest => dest.Comments, opt => opt.Ignore())
      .ForMember(dest => dest.StatusHistory, opt => opt.Ignore());

    CreateMap<JobApplication, backend.DTOs.JobApplication.JobApplicationResponseDto>()
      .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
      .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => "Internal"))
      .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate.FullName))
      .ForMember(dest => dest.CandidateEmail, opt => opt.MapFrom(src => src.Candidate.User.Email))
      .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Status))
      .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.UserName : null))
      .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedByUser != null ? src.UpdatedByUser.UserName : null))
      .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.ApplicationDocuments))
      .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
      .ForMember(dest => dest.StatusHistory, opt => opt.MapFrom(src => src.StatusHistory));

    CreateMap<JobApplication, backend.DTOs.JobApplication.JobApplicationListDto>()
      .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
      .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate.FullName))
      .ForMember(dest => dest.CandidateEmail, opt => opt.MapFrom(src => src.Candidate.User.Email))
      .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Status));

    // Screening mappings
    CreateMap<JobApplication, backend.DTOs.Screening.ScreeningResponseDto>()
      .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate.FullName ?? ""))
      .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Status))
      .ForMember(dest => dest.ScreenedBy, opt => opt.MapFrom(src => src.UpdatedByUser != null ? src.UpdatedByUser.UserName : ""))
      .ForMember(dest => dest.ScreenedAt, opt => opt.MapFrom(src => src.LastUpdated ?? DateTime.UtcNow));

    CreateMap<CandidateSkill, backend.DTOs.Screening.CandidateSkillScreeningDto>()
      .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.SkillName))
      .ForMember(dest => dest.YearsOfExperience, opt => opt.MapFrom(src => src.YearOfExperience))
      .ForMember(dest => dest.ProficiencyLevel, opt => opt.MapFrom(src => ""))
      .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.YearOfExperience.HasValue));

    CreateMap<Comment, backend.DTOs.Screening.CommentResponseDto>()
      .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.CommentText))
      .ForMember(dest => dest.CommenterName, opt => opt.MapFrom(src => src.Commenter.FullName ?? ""))
      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt ?? DateTime.UtcNow));

    CreateMap<JobApplication, backend.DTOs.Screening.ShortlistResponseDto>()
      .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate.FullName ?? ""))
      .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Status))
      .ForMember(dest => dest.ShortlistedAt, opt => opt.MapFrom(src => src.LastUpdated ?? DateTime.UtcNow))
      .ForMember(dest => dest.ShortlistedBy, opt => opt.MapFrom(src => src.UpdatedByUser != null ? src.UpdatedByUser.UserName : ""));

    // Interview mappings
    CreateMap<Interview, InterviewDto>();
    CreateMap<CreateInterviewDto, Interview>();

    CreateMap<InterviewSchedule, InterviewScheduleDto>();
    CreateMap<CreateInterviewScheduleDto, InterviewSchedule>();

    CreateMap<InterviewFeedback, InterviewFeedbackDto>();
    CreateMap<CreateInterviewFeedbackDto, InterviewFeedback>();

    // Event mappings
    CreateMap<backend.DTOs.Event.CreateEventDto, Event>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
      .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

    CreateMap<backend.DTOs.Event.UpdateEventDto, Event>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
      .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
      .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
      .ForMember(dest => dest.EventCandidates, opt => opt.Ignore());

    CreateMap<Event, backend.DTOs.Event.EventDto>()
      .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByEmployee != null ? src.CreatedByEmployee.FullName : null));

    CreateMap<Event, backend.DTOs.Event.EventListDto>()
      .ForMember(dest => dest.CandidateCount, opt => opt.MapFrom(src => src.EventCandidates.Count));

    CreateMap<EventCandidate, backend.DTOs.Event.EventCandidateDto>()
      .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event != null ? src.Event.Name : null))
      .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate.FullName))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status != null ? src.Status.Status : null));

    // Verification mappings
    CreateMap<backend.DTOs.Verification.CreateVerificationDto, Verification>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
      .ForMember(dest => dest.VerifiedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

    CreateMap<backend.DTOs.Verification.UpdateVerificationDto, Verification>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.DocumentId, opt => opt.Ignore())
      .ForMember(dest => dest.CandidateId, opt => opt.Ignore())
      .ForMember(dest => dest.VerifiedAt, opt => opt.Ignore())
      .ForMember(dest => dest.Document, opt => opt.Ignore())
      .ForMember(dest => dest.Candidate, opt => opt.Ignore())
      .ForMember(dest => dest.VerifiedByEmployee, opt => opt.Ignore())
      .ForMember(dest => dest.Status, opt => opt.Ignore());

    CreateMap<Verification, backend.DTOs.Verification.VerificationDto>()
      .ForMember(dest => dest.DocumentUrl, opt => opt.MapFrom(src => src.Document != null ? src.Document.Url : string.Empty))
      .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.Document != null && src.Document.DocumentType != null ? src.Document.DocumentType.Type : string.Empty))
      .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate != null ? src.Candidate.FullName : null))
      .ForMember(dest => dest.VerifiedByName, opt => opt.MapFrom(src => src.VerifiedByEmployee != null ? src.VerifiedByEmployee.FullName : null))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status != null ? src.Status.Status : null));

    CreateMap<Verification, backend.DTOs.Verification.VerificationListDto>()
      .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate != null ? src.Candidate.FullName : null))
      .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.Document != null && src.Document.DocumentType != null ? src.Document.DocumentType.Type : string.Empty))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status != null ? src.Status.Status : null));

    // Notification mappings
    CreateMap<Notification, backend.DTOs.Notification.NotificationDto>();

    // Scoring mappings
    CreateMap<ScoringConfiguration, ScoringConfigurationDto>();
    CreateMap<CreateScoringConfigurationDto, ScoringConfiguration>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
      .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
      .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
      .ForMember(dest => dest.Position, opt => opt.Ignore());

    CreateMap<AutomatedScore, AutomatedScoreDto>()
      .ForMember(dest => dest.CandidateName, opt => opt.Ignore())
      .ForMember(dest => dest.JobTitle, opt => opt.Ignore());
  }
}
