using AutoMapper;
using Backend.Dtos.JobApplications;
using Backend.Models;

namespace Backend.Mappings;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<JobApplication, JobApplicationDto>()
        .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job!.Title))
        .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate!.FullName ?? "Unknown"))
        .ForMember(dest => dest.CandidateEmail, opt => opt.MapFrom(src => src.Candidate!.User!.Email));

    CreateMap<JobApplicationCreateDto, JobApplication>();
    CreateMap<JobApplicationUpdateDto, JobApplication>();
  }
}
