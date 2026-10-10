using AutoMapper;
using MTK.Modules.Hr.Application.VacationApplications.GetVacationApplicationById;
using MTK.Modules.Hr.Application.VacationApplications.SearchVacationApplications;
using MTK.Modules.Hr.Domain.VacationApplications;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class VacationApplicationProfile : Profile
{
    public VacationApplicationProfile()
    {
        CreateMap<VacationApplication, GetVacationApplicationByIdResponse>()
            .ForMember(dest => dest.ReturnToWorkDate, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.VacationOrder, opt => opt.MapFrom(src => src.VacationOrder))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<VacationApplication, SearchVacationApplicationsResponseItem>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.Order,
            opt => opt.MapFrom(src => src.VacationOrder));
    }
}
