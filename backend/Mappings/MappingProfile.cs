using AutoMapper;
using Backend.Dtos.JobApplications;
using Backend.Dtos.Candidates;
using Backend.Dtos.Jobs;
using Backend.Dtos.Users;
using Backend.Dtos.Roles;
using Backend.Dtos.Employees;
using Backend.Dtos.Addresses;
using Backend.Dtos.Auth;
using Backend.Dtos.Positions;
using Backend.Dtos;
using Backend.Models;

namespace Backend.Mappings;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    // Job Application Mappings
    CreateMap<JobApplication, JobApplicationDto>()
        .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job!.Title))
        .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate!.FullName ?? "Unknown"))
        .ForMember(dest => dest.CandidateEmail, opt => opt.MapFrom(src => src.Candidate!.User!.Email))
        .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status!.Status))
        .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => 
            src.ReviewedBy != null 
                ? src.Job!.Recruiter!.FullName ?? "Unknown" 
                : null));

    CreateMap<JobApplicationCreateDto, JobApplication>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.AppliedAt, opt => opt.Ignore())
        .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
        .ForMember(dest => dest.ReviewedAt, opt => opt.Ignore())
        .ForMember(dest => dest.ReviewedBy, opt => opt.Ignore());

    CreateMap<JobApplicationUpdateDto, JobApplication>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.JobId, opt => opt.Ignore())
        .ForMember(dest => dest.CandidateId, opt => opt.Ignore())
        .ForMember(dest => dest.AppliedAt, opt => opt.Ignore())
        .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow));

    // Candidate Mappings
    CreateMap<Candidate, CandidateDto>()
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User!.Email))
        .ForMember(dest => dest.AddressDetails, opt => opt.MapFrom(src =>
            src.Address != null
                ? $"{src.Address.AddressLine1}, {src.Address.City!.Name}, {src.Address.City.State!.Name} {src.Address.Pincode}"
                : null))
        .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.CandidateSkills))
        .ForMember(dest => dest.TotalApplications, opt => opt.MapFrom(src => src.JobApplications.Count));

    CreateMap<CandidateCreateDto, Candidate>()
        .ForMember(dest => dest.Id, opt => opt.Ignore());

    CreateMap<CandidateUpdateDto, Candidate>()
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    CreateMap<CreateCandidateDto, Candidate>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.User, opt => opt.Ignore())
        .ForMember(dest => dest.Address, opt => opt.Ignore())
        .ForMember(dest => dest.CandidateSkills, opt => opt.Ignore())
        .ForMember(dest => dest.CandidateQualifications, opt => opt.Ignore());

    CreateMap<UpdateCandidateDto, Candidate>()
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    // Candidate Skill Mappings
    CreateMap<CandidateSkill, CandidateSkillDto>()
        .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill!.Name));

    CreateMap<CandidateSkillCreateDto, CandidateSkill>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.CandidateId, opt => opt.Ignore());

    CreateMap<CandidateSkillUpdateDto, CandidateSkill>()
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    // Job Mappings
    CreateMap<Job, JobDto>()
        .ForMember(dest => dest.JobTypeName, opt => opt.MapFrom(src => src.JobType!.Type))
        .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Position!.Title))
        .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status!.Status))
        .ForMember(dest => dest.LocationDetails, opt => opt.MapFrom(src =>
            src.Location != null
                ? $"{src.Location.AddressLine1}, {src.Location.City!.Name}, {src.Location.City.State!.Name}"
                : null))
        .ForMember(dest => dest.RecruiterName, opt => opt.MapFrom(src => src.Recruiter!.FullName));

    CreateMap<JobCreateDto, Job>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

    CreateMap<JobUpdateDto, Job>()
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    // User Mappings
    CreateMap<User, UserDto>()
        .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.Name));

    CreateMap<UserCreateDto, User>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

    CreateMap<UserUpdateDto, User>()
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForMember(dest => dest.Password, opt => opt.MapFrom(src =>
            !string.IsNullOrEmpty(src.Password) ? BCrypt.Net.BCrypt.HashPassword(src.Password) : null))
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    // Role Mappings
    CreateMap<Role, RoleDto>();
    CreateMap<RoleCreateDto, Role>()
        .ForMember(dest => dest.Id, opt => opt.Ignore());
    CreateMap<RoleUpdateDto, Role>()
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    // Employee Mappings
    CreateMap<Employee, EmployeeDto>()
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User!.Email))
        .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.User!.Role!.Name))
        .ForMember(dest => dest.BranchDetails, opt => opt.MapFrom(src =>
            src.BranchAddress != null
                ? $"{src.BranchAddress.AddressLine1}, {src.BranchAddress.City!.Name}"
                : null));

    CreateMap<EmployeeCreateDto, Employee>()
        .ForMember(dest => dest.Id, opt => opt.Ignore());

    CreateMap<EmployeeUpdateDto, Employee>()
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    // Address Mappings
    CreateMap<Address, AddressDto>()
        .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City!.Name))
        .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.City!.State!.Name))
        .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.City!.State!.Country!.Name));

    CreateMap<AddressCreateDto, Address>()
        .ForMember(dest => dest.Id, opt => opt.Ignore());

    CreateMap<AddressUpdateDto, Address>()
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    // Auth Mappings
    CreateMap<RegisterRequestDto, User>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

    // Position Mappings
    CreateMap<Position, PositionDto>()
        .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status!.Status))
        .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer!.FullName));

    CreateMap<PositionCreateDto, Position>()
        .ForMember(dest => dest.Id, opt => opt.Ignore());

    CreateMap<PositionUpdateDto, Position>()
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    // Lookup entity mappings
    CreateMap<Skill, SkillDto>();
    CreateMap<JobType, JobTypeDto>();
    CreateMap<StatusType, StatusTypeDto>();
    CreateMap<Qualification, QualificationDto>();
    CreateMap<City, CityDto>()
        .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State!.Name));
    CreateMap<State, StateDto>()
        .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country!.Name));
    CreateMap<Country, CountryDto>();
  }
}
