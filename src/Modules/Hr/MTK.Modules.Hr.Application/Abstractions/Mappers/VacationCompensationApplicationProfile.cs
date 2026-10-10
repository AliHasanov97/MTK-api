using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.VacationCompensationApplications.CreateVacationCompensationApplication;
using MTK.Modules.Hr.Application.VacationCompensationApplications.GetVacationCompensationApplicationById;
using MTK.Modules.Hr.Application.VacationCompensationApplications.SearchVacationCompensationApplications;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class VacationCompensationApplicationProfile : Profile
{
    public VacationCompensationApplicationProfile()
    {
        // ResponseObjectWithName mapping (POST endpoint üçün sadə response)
        CreateMap<VacationCompensationApplication, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApplicationNumber.ToString()));

        CreateMap<VacationCompensationApplication, CreateVacationCompensationApplicationResponse>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee.Name} {src.Employee.Surname}"))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => $"{src.CreatedBy.FirstName} {src.CreatedBy.LastName}"));

        CreateMap<VacationCompensationApplication, GetVacationCompensationApplicationByIdResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.CompensationOrder, opt => opt.MapFrom(src => src.CompensationOrder))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<VacationCompensationApplication, SearchVacationCompensationApplicationsResponseItem>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.Order,
            opt => opt.MapFrom(src => src.CompensationOrder));
    }
}