using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.OrdersForChangeOfPosition.GetOrderForChangeOfPositionById;
using MTK.Modules.Hr.Application.OrdersForChangeOfPosition.SearchOrdersForChangeOfPosition;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class OrderForChangeOfPositionProfile : Profile
{
    public OrderForChangeOfPositionProfile()
    {
        CreateMap<OrderForChangeOfPosition, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<OrderForChangeOfPosition, GetOrderForChangeOfPositionByIdResponse>();

        CreateMap<OrderForChangeOfPosition, SearchOrdersForChangeOfPositionResponseItem>();
    }
}
