using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.GetEmploymentStatusChangeApplicationById;
using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.SearchEmploymentStatusChangeApplications;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class EmploymentStatusChangeApplicationProfile : Profile
{
    public EmploymentStatusChangeApplicationProfile()
    {
        // ResponseObjectWithName mapping (POST endpoint üçün sadə response)
        CreateMap<EmploymentStatusChangeApplication, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApplicationNumber.ToString()));

        CreateMap<EmploymentStatusChangeApplication, GetEmploymentStatusChangeApplicationByIdResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.NewEmploymentType, opt => opt.MapFrom(src => src.NewEmploymentType.ToString()))
            .ForMember(dest => dest.CurrentEmploymentType, opt => opt.MapFrom(src => src.CurrentEmploymentType.ToString()))
            .ForMember(dest => dest.OrderExecutionSupervisor, opt => opt.MapFrom(src => src.OrderExecutionSupervisor))
            .ForMember(dest => dest.EmploymentStatusChangeOrder, opt => opt.MapFrom(src => src.EmploymentStatusChangeOrder))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<EmploymentStatusChangeApplication, SearchEmploymentStatusChangeApplicationsResponseItem>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.NewEmploymentType, opt => opt.MapFrom(src => src.NewEmploymentType.ToString()))
            .ForMember(dest => dest.CurrentEmploymentType, opt => opt.MapFrom(src => src.CurrentEmploymentType.ToString()))
            .ForMember(dest => dest.OrderExecutionSupervisor, opt => opt.MapFrom(src => src.OrderExecutionSupervisor))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.EmploymentStatusChangeOrder));
    }
}
