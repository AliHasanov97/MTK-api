using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.Orders.SearchOrders;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // Ərizənin RelatedOrder-i əmrin əsas tipi ilə gəlir (VacationOrder, BonusOrder, ...): Id + əmr nömrəsi
        CreateMap<Order, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<Order, SearchOrdersResponseItem>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.RelatedEmployee))
            .ForMember(dest => dest.JobApplicant, opt => opt.MapFrom(src => src.RelatedJobApplicant))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
    }
}