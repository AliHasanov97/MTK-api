using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.VacationReturnOrders.GetVacationReturnOrderById;
using MTK.Modules.Hr.Application.VacationReturnOrders.SearchVacationReturnOrders;
using MTK.Modules.Hr.Domain.VacationReturnOrders;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class VacationReturnOrderProfile : Profile
{
    public VacationReturnOrderProfile()
    {
        // ResponseObjectWithName mapping
        CreateMap<VacationReturnOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<VacationReturnOrder, GetVacationReturnOrderByIdResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<VacationReturnOrder, SearchVacationReturnOrdersResponseItem>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
    }
}
