using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.ConvertToOrder;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.GetUnpaidLeaveApplicationById;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.SearchUnpaidLeaveApplications;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class UnpaidLeaveApplicationProfile : Profile
{
    public UnpaidLeaveApplicationProfile()
    {
        // ResponseObjectWithName mapping (POST endpoint üçün sadə response)
        CreateMap<UnpaidLeaveApplication, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApplicationNumber.ToString()));

        CreateMap<UnpaidLeaveApplication, GetUnpaidLeaveApplicationByIdResponse>()
            .ForMember(dest => dest.ReturnToWorkDate, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.UnpaidLeaveOrder, opt => opt.MapFrom(src => src.UnpaidLeaveOrder))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<UnpaidLeaveApplication, SearchUnpaidLeaveApplicationsResponseItem>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.Order,
            opt => opt.MapFrom(src => src.UnpaidLeaveOrder));
    }
}
