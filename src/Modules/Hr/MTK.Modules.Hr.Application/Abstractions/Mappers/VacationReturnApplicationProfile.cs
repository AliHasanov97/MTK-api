using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.VacationReturnApplications.GetVacationReturnApplicationById;
using MTK.Modules.Hr.Application.VacationReturnApplications.SearchVacationReturnApplications;
using MTK.Modules.Hr.Domain.VacationReturnApplications;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class VacationReturnApplicationProfile : Profile
{
    public VacationReturnApplicationProfile()
    {
        // ResponseObjectWithName mapping (POST endpoint üçün sadə response)
        CreateMap<VacationReturnApplication, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApplicationNumber.ToString()));

        CreateMap<VacationReturnApplication, GetVacationReturnApplicationByIdResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.VacationReturnOrder, opt => opt.MapFrom(src => src.VacationReturnOrder))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<VacationReturnApplication, SearchVacationReturnApplicationsResponseItem>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.VacationReturnOrder));
    }
}
