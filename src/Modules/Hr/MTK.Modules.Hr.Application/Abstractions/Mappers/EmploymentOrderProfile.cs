using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.EmploymentOrders.GetEmploymentOrderById;
using MTK.Modules.Hr.Application.EmploymentOrders.SearchEmploymentOrders;
using MTK.Modules.Hr.Domain.EmploymentOrders;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class EmploymentOrderProfile : Profile
{
    public EmploymentOrderProfile()
    {
        CreateMap<EmploymentOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));
        
        CreateMap<EmploymentOrder, GetEmploymentOrderByIdResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.JobApplication))
            .ForMember(dest => dest.Job, opt => opt.MapFrom(src => src.JobApplication.Job));

        CreateMap<EmploymentOrder, SearchEmploymentOrdersResponseItem>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.JobApplication))
            .ForMember(dest => dest.Job, opt => opt.MapFrom(src => src.JobApplication.Job));
    }
}
