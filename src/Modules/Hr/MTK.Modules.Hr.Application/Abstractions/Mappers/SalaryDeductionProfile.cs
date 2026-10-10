using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.SalaryDeductions.GetSalaryDeductionById;
using MTK.Modules.Hr.Application.SalaryDeductions.SearchSalaryDeductions;
using MTK.Modules.Hr.Domain.SalaryDeductions;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class SalaryDeductionProfile : Profile
{
    public SalaryDeductionProfile()
    {
        CreateMap<SalaryDeduction, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<SalaryDeduction, GetSalaryDeductionByIdResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<SalaryDeduction, SearchSalaryDeductionsResponseItem>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
    }
}
