using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.GetEmploymentStatusChangeOrderById;
using MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.SearchEmploymentStatusChangeOrders;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class EmploymentStatusChangeOrderProfile : Profile
{
    public EmploymentStatusChangeOrderProfile()
    {
        // ResponseObjectWithName mapping
        CreateMap<EmploymentStatusChangeOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<EmploymentStatusChangeOrder, GetEmploymentStatusChangeOrderByIdResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.NewEmploymentType, opt => opt.MapFrom(src => src.NewEmploymentType.ToString()))
            .ForMember(dest => dest.OrderExecutionSupervisor, opt => opt.MapFrom(src => src.OrderExecutionSupervisor))
            .ForMember(dest => dest.Application, opt => opt.MapFrom(src => src.EmploymentStatusChangeApplication))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<EmploymentStatusChangeOrder, SearchEmploymentStatusChangeOrdersResponseItem>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.NewEmploymentType, opt => opt.MapFrom(src => src.NewEmploymentType.ToString()))
            .ForMember(dest => dest.OrderExecutionSupervisor, opt => opt.MapFrom(src => src.OrderExecutionSupervisor))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
    }
}
