using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.CompensationOrders.GetCompensationOrderById;
using MTK.Modules.Hr.Application.CompensationOrders.SearchCompensationOrders;
using MTK.Modules.Hr.Application.VacationCompensationApplications.ConvertToOrder;
using MTK.Modules.Hr.Domain.CompensationOrders;
using MTK.Modules.Hr.Domain.VacationBalances;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class CompensationOrderProfile : Profile
{
    public CompensationOrderProfile()
    {
        CreateMap<CompensationOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));
        
        CreateMap<CompensationOrder, ConvertVacationCompensationToOrderResponse>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee));

        CreateMap<CompensationOrder, SearchCompensationOrdersResponseItem>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        // VacationBalanceInfo-nin nested map-i. VacationCompensationApplicationProfile da eyni
        // DTO-dan istifadə etdiyi üçün tək yerdə təyin olunur (təkrar olsa AutoMapper
        // DuplicateTypeMapConfigurationException atır).
        CreateMap<VacationBalance, VacationBalanceInfo>();

        CreateMap<CompensationOrder, GetCompensationOrderByIdResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.VacationBalance, opt => opt.MapFrom(src => src.VacationBalance))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
    }
}