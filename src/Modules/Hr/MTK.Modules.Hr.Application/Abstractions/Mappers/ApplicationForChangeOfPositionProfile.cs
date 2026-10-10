using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.AddApplicationForChangeOfPosition;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.GetApplicationForChangeOfPositionById;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.SearchApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.UpdateApplicationForChangeOfPosition;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class ApplicationForChangeOfPositionProfile : Profile
{
    public ApplicationForChangeOfPositionProfile()
    {
        CreateMap<ApplicationForChangeOfPosition, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApplicationNumber.ToString()));

        CreateMap<ApplicationForChangeOfPosition, AddApplicationForChangeOfPositionResponse>();
        CreateMap<ApplicationForChangeOfPosition, UpdateApplicationForChangeOfPositionResponse>();

        CreateMap<ApplicationForChangeOfPosition, GetApplicationForChangeOfPositionByIdResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OrderForChangeOfPosition, opt => opt.MapFrom(src => src.OrderForChangeOfPosition));

        CreateMap<ApplicationForChangeOfPosition, SearchApplicationsForChangeOfPositionResponseItem>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Order,
                opt => opt.MapFrom(src => src.OrderForChangeOfPosition));   
    }
}
