using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.Applications.SearchApplications;
using ApplicationEntity = MTK.Modules.Hr.Domain.Applications.Application;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<ApplicationEntity, SearchApplicationsResponseItem>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.RelatedEmployee))
            .ForMember(dest => dest.JobApplicant, opt => opt.MapFrom(src => src.RelatedJobApplicant))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.RelatedOrder));
    }
}