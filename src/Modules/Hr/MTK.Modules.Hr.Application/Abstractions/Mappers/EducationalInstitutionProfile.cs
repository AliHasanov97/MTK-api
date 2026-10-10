using AutoMapper;
using MTK.Modules.Hr.Application.EducationalInstitutions.CreateEducationalInstitution;
using MTK.Modules.Hr.Application.EducationalInstitutions.GetEducationalInstitutionById;
using MTK.Modules.Hr.Application.EducationalInstitutions.SearchEducationalInstitutions;
using MTK.Modules.Hr.Application.EducationalInstitutions.UpdateEducationalInstitution;
using MTK.Modules.Hr.Domain.EducationalInstitutions;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class EducationalInstitutionProfile : Profile
{
    public EducationalInstitutionProfile()
    {
        CreateMap<EducationalInstitution, CreateEducationalInstitutionResponse>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        CreateMap<EducationalInstitution, UpdateEducationalInstitutionResponse>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        CreateMap<EducationalInstitution, SearchEducationalInstitutionsResponseItem>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        CreateMap<EducationalInstitution, GetEducationalInstitutionByIdResponse>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
    }
}
