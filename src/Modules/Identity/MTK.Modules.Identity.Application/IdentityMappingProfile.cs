using AutoMapper;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Application.AuditLogs.SearchAuditLogs;
using MTK.Modules.Identity.Application.Groups.GetGroupById;
using MTK.Modules.Identity.Application.Groups.GetGroups;
using MTK.Modules.Identity.Application.Groups.SearchGroups;
using MTK.Modules.Identity.Application.Users.GetAllUsers;
using MTK.Modules.Identity.Application.Users.GetUserById;
using MTK.Modules.Identity.Application.Users.SearchUsers;
using MTK.Modules.Identity.Domain.AuditLogs;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application;

/// <summary>
/// Covers only the "pure 1:1" query-response mappings. Handlers that combine a User
/// lookup with a separate Keycloak role/group call (GetUserRoles, GetUserGroups,
/// GetUserRolesAndGroups) stay hand-written — two sources, not one.
/// </summary>
// "Profile" is ambiguous here — MTK.Modules.Identity.Application.Profile (the
// ChangeUserInfo feature's namespace) shadows AutoMapper.Profile, so this must be
// fully qualified.
public sealed class IdentityMappingProfile : AutoMapper.Profile
{
    public IdentityMappingProfile()
    {
        // These responses are immutable records (positional ctor, no settable
        // properties), so a renamed/computed member must be wired as a
        // constructor-parameter mapping (ForCtorParam) — plain ForMember makes
        // AutoMapper fall back to "construct via parameterless ctor + set
        // properties", which fails at runtime for a type with no such ctor
        // ("needs to have a constructor with 0 args or only optional args").

        // GetCurrentUser's own UserResponse — sourced from the request-scoped
        // IUserContext, not a Domain entity. Id has no same-named source member.
        CreateMap<IUserContext, Users.GetCurrentUser.UserResponse>()
            .ForCtorParam(nameof(Users.GetCurrentUser.UserResponse.Id), o => o.MapFrom(s => s.UserId));

        // User.Status (enum) -> string maps via AutoMapper's built-in converter.
        CreateMap<User, UserResponse>();
        CreateMap<User, UserDetailResponse>();
        CreateMap<User, UserSearchResult>();

        CreateMap<GroupDto, GroupResponse>();
        CreateMap<GroupDto, GroupSearchResult>();
        CreateMap<GroupDto, GroupDetailResponse>();

        CreateMap<AuditLog, AuditLogDto>()
            .ForCtorParam(nameof(AuditLogDto.Timestamp), o => o.MapFrom(s => s.Timestamp.ToUniversalTime().Date));
    }
}
