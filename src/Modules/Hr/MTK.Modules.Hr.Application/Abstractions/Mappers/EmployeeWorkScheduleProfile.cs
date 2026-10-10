using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.EmployeeWorkSchedules.GetEmployeeWorkSchedule;
using MTK.Modules.Hr.Application.EmployeeWorkSchedules.SetEmployeeWorkSchedule;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class EmployeeWorkScheduleProfile : Profile
{
    public EmployeeWorkScheduleProfile()
    {
        CreateMap<EmployeeWorkSchedule, SetEmployeeWorkScheduleResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src =>
                ResponseObjectWithName.Create(
                    src.Employee.Id,
                    $"{src.Employee.Name} {src.Employee.Surname} {src.Employee.FathersName}")));

        CreateMap<EmployeeWorkSchedule, GetEmployeeWorkScheduleResponse>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src =>
                ResponseObjectWithName.Create(
                    src.Employee.Id,
                    $"{src.Employee.Name} {src.Employee.Surname} {src.Employee.FathersName}")));
    }
}
