using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.GetWorkOnNonWorkdayOrderById;
using MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.SearchWorkOnNonWorkdayOrders;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class WorkOnNonWorkdayOrderProfile : Profile
{
    public WorkOnNonWorkdayOrderProfile()
    {
        CreateMap<WorkOnNonWorkdayOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<WorkOnNonWorkdayOrder, GetWorkOnNonWorkdayOrderByIdResponse>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<WorkOnNonWorkdayOrder, SearchWorkOnNonWorkdayOrdersResponseItem>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
    }
}
