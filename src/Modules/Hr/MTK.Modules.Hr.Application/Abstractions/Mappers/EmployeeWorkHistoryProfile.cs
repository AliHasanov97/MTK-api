using AutoMapper;
using MTK.Modules.Hr.Application.EmployeeWorkHistories.AddEmployeeWorkHistory;
using MTK.Modules.Hr.Application.EmployeeWorkHistories.GetEmployeeWorkHistories;
using MTK.Modules.Hr.Application.EmployeeWorkHistories.UpdateEmployeeWorkHistory;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class EmployeeWorkHistoryProfile : Profile
{
    public EmployeeWorkHistoryProfile()
    {
        CreateMap<EmployeeWorkHistory, AddEmployeeWorkHistoryResponse>();
        CreateMap<EmployeeWorkHistory, UpdateEmployeeWorkHistoryResponse>();
        CreateMap<EmployeeWorkHistory, EmployeeWorkHistoryItem>()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.CalculateDuration()));
    }
}
