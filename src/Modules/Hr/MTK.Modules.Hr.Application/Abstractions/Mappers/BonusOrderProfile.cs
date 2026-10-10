using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.BonusOrders.GetBonusOrderById;
using MTK.Modules.Hr.Application.BonusOrders.SearchBonusOrders;
using MTK.Modules.Hr.Domain.BonusOrders;
using MTK.Modules.Hr.Domain.FileAttachments;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class BonusOrderProfile : Profile
{
    public BonusOrderProfile()
    {
        CreateMap<BonusOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<BonusOrder, GetBonusOrderByIdResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.OrderExecutionSupervisor, opt => opt.MapFrom(src => src.OrderExecutionSupervisor))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<BonusOrder, SearchBonusOrdersResponseItem>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.OrderExecutionSupervisor, opt => opt.MapFrom(src => src.OrderExecutionSupervisor))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

    }
}
