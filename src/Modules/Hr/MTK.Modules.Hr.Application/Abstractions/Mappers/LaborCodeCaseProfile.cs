using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.LaborCodeCases.SearchLaborCodeCases;
using MTK.Modules.Hr.Domain.LaborCodeCases;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class LaborCodeCaseProfile : Profile
{
    public LaborCodeCaseProfile()
    {
        CreateMap<LaborCodeCase, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Code + " - " + src.Name));

        CreateMap<LaborCodeCase, SearchLaborCodeCasesResponseItem>()
            .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));
    }
}
