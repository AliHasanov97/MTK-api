using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.EducationLeaveApplications.ConvertToOrder;
using MTK.Modules.Hr.Application.EducationLeaveApplications.GetEducationLeaveApplicationById;
using MTK.Modules.Hr.Application.EducationLeaveApplications.SearchEducationLeaveApplications;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class EducationLeaveApplicationProfile : Profile
{
    public EducationLeaveApplicationProfile()
    {
        // ResponseObjectWithName mapping (POST endpoint üçün sadə response)
        CreateMap<EducationLeaveApplication, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApplicationNumber.ToString()));

        CreateMap<EducationLeaveApplication, GetEducationLeaveApplicationByIdResponse>()
            .ForMember(dest => dest.ReturnToWorkDate, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.EducationLeaveOrder))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<EducationLeaveApplication, SearchEducationLeaveApplicationsResponseItem>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.Order,
            opt => opt.MapFrom(src => src.EducationLeaveOrder));
    }
}
