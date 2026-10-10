using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.EducationLeaveApplications.ConvertToOrder;
using MTK.Modules.Hr.Application.EducationLeaveOrders.GetEducationLeaveOrderById;
using MTK.Modules.Hr.Application.EducationLeaveOrders.SearchEducationLeaveOrders;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class EducationLeaveOrderProfile : Profile
{
    public EducationLeaveOrderProfile()
    {
        CreateMap<EducationLeaveOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<EducationLeaveOrder, ConvertEducationLeaveToOrderResponse>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<EducationLeaveOrder, GetEducationLeaveOrderByIdResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<EducationLeaveOrder, SearchEducationLeaveOrdersResponseItem>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
    }
}
