using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.UnexcusedAbsences.AddUnexcusedAbsence;
using MTK.Modules.Hr.Application.UnexcusedAbsences.GetUnexcusedAbsenceById;
using MTK.Modules.Hr.Application.UnexcusedAbsences.SearchUnexcusedAbsences;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class UnexcusedAbsenceProfile : Profile
{
    public UnexcusedAbsenceProfile()
    {
        CreateMap<UnexcusedAbsenceOrder, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderNumber.ToString()));

        CreateMap<UnexcusedAbsenceOrder, AddUnexcusedAbsenceResponse>();
        CreateMap<UnexcusedAbsenceOrder, GetUnexcusedAbsenceByIdResponse>();
        CreateMap<UnexcusedAbsenceOrder, SearchUnexcusedAbsencesResponseItem>();
    }
}
