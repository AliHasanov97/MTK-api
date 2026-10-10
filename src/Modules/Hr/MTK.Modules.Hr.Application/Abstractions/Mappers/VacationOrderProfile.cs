using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.VacationOrders.GetVacationOrderById;
using MTK.Modules.Hr.Application.VacationOrders.SearchVacationOrders;
using MTK.Modules.Hr.Domain.VacationOrders;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class VacationOrderProfile : Profile
{
    public VacationOrderProfile()
    {
        CreateMap<VacationOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<VacationOrder, GetVacationOrderByIdResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<VacationOrder, SearchVacationOrdersResponseItem>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
    }
}
