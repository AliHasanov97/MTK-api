using AutoMapper;
using MTK.Modules.Hr.Application.EmployeeEducationHistories.AddEmployeeEducationHistory;
using MTK.Modules.Hr.Application.EmployeeEducationHistories.GetEmployeeEducationHistories;
using MTK.Modules.Hr.Application.EmployeeEducationHistories.SearchEmployeeEducationHistories;
using MTK.Modules.Hr.Application.EmployeeEducationHistories.UpdateEmployeeEducationHistory;
using MTK.Modules.Hr.Domain.EmployeeEducationHistories;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class EmployeeEducationHistoryProfile : Profile
{
    public EmployeeEducationHistoryProfile()
    {
        CreateMap<EmployeeEducationHistory, AddEmployeeEducationHistoryResponse>()
            .ForMember(dest => dest.EducationalInstitutionName,
                opt => opt.MapFrom(src => src.EducationalInstitution != null ? src.EducationalInstitution.Name : null));

        CreateMap<EmployeeEducationHistory, UpdateEmployeeEducationHistoryResponse>()
            .ForMember(dest => dest.EducationalInstitutionName,
                opt => opt.MapFrom(src => src.EducationalInstitution != null ? src.EducationalInstitution.Name : null));

        CreateMap<EmployeeEducationHistory, EmployeeEducationHistoryItem>()
            .ForMember(dest => dest.EducationalInstitutionName,
                opt => opt.MapFrom(src => src.EducationalInstitution != null ? src.EducationalInstitution.Name : null));

        CreateMap<EmployeeEducationHistory, SearchEmployeeEducationHistoriesResponseItem>()
            .ForMember(dest => dest.EducationalInstitutionName,
                opt => opt.MapFrom(src => src.EducationalInstitution != null ? src.EducationalInstitution.Name : null));
    }
}
