using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.Warnings.AddWarning;
using MTK.Modules.Hr.Application.Warnings.GetWarningById;
using MTK.Modules.Hr.Application.Warnings.SearchWarnings;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class WarningProfile : Profile
{
    public WarningProfile()
    {
        CreateMap<WarningOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<WarningOrder, AddWarningResponse>();
        CreateMap<WarningOrder, GetWarningByIdResponse>();
        CreateMap<WarningOrder, SearchWarningsResponseItem>();
    }
}
