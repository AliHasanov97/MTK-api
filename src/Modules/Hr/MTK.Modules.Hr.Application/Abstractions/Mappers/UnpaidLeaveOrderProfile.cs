using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.ConvertToOrder;
using MTK.Modules.Hr.Application.UnpaidLeaveOrders.GetUnpaidLeaveOrderById;
using MTK.Modules.Hr.Application.UnpaidLeaveOrders.SearchUnpaidLeaveOrders;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class UnpaidLeaveOrderProfile : Profile
{
    public UnpaidLeaveOrderProfile()
    {
        CreateMap<UnpaidLeaveOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));
        
        CreateMap<UnpaidLeaveOrder, ConvertUnpaidLeaveToOrderResponse>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<UnpaidLeaveOrder, GetUnpaidLeaveOrderByIdResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<UnpaidLeaveOrder, SearchUnpaidLeaveOrdersResponseItem>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
    }
}
