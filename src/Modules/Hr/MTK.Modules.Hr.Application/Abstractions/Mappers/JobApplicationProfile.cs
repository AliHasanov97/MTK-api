using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.JobApplications.AddJobApplication;
using MTK.Modules.Hr.Application.JobApplications.GetJobApplicationById;
using MTK.Modules.Hr.Application.JobApplications.SearchJobApplications;
using MTK.Modules.Hr.Application.JobApplications.UpdateJobApplication;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class JobApplicationProfile : Profile
{
    public JobApplicationProfile()
    {
        CreateMap<JobApplication, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.Name} {src.Surname} {src.FathersName} {(src.Gender == Gender.Male ? "oğlu" : "qızı")}"));

        CreateMap<JobApplication, AddJobApplicationResponse>();
        CreateMap<JobApplication, UpdateJobApplicationResponse>();
        CreateMap<JobApplication, GetJobApplicationByIdResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.EmploymentOrder, opt => opt.MapFrom(src => src.EmploymentOrder));
        CreateMap<JobApplication, SearchJobApplicationsResponseItem>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Order,
                opt => opt.MapFrom(src => src.EmploymentOrder));
    }
}