using AutoMapper;
using MTK.Modules.Hr.Application.CalendarDays;
using MTK.Modules.Hr.Domain.CalendarDays;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class CalendarDayProfile : Profile
{
    public CalendarDayProfile()
    {
        CreateMap<CalendarDay, CalendarDayDto>();
    }
}
