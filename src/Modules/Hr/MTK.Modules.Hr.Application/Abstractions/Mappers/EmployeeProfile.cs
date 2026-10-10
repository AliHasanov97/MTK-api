using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.Employees.AddEmployee;
using MTK.Modules.Hr.Application.Employees.GetEmployeeById;
using MTK.Modules.Hr.Application.Employees.SearchEmployees;
using MTK.Modules.Hr.Application.Employees.UpdateEmployee;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.Name} {src.Surname} {src.FathersName} {(src.Gender == Gender.Male ? "oğlu" : "qızı")}"));

        CreateMap<Employee, AddEmployeeResponse>();
        CreateMap<Employee, GetEmployeeByIdResponse>()
            .ForMember(dest => dest.TotalWorkExperience, opt => opt.Ignore())
            .ForMember(dest => dest.OrganizationWorkExperience, opt => opt.Ignore());
        CreateMap<Employee, UpdateEmployeeResponse>()
            .ForMember(dest => dest.TotalWorkExperience, opt => opt.Ignore())
            .ForMember(dest => dest.OrganizationWorkExperience, opt => opt.Ignore());
        CreateMap<Employee, SearchEmployeesResponseItem>()
            .ForMember(dest => dest.Age, opt => opt.Ignore());
    }
}
