using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Domain.Users;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => (src.FirstName + " " + src.LastName).Trim()));
    }
}
