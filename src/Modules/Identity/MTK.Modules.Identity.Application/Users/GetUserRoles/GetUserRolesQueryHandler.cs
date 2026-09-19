using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.GetUserRoles;

internal sealed class GetUserRolesQueryHandler : IQueryHandler<GetUserRolesQuery, UserRolesResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserRolesQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserRolesResponse>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserRolesResponse>(UserErrors.NotFound(request.UserId));
        }

        // Get direct roles
        var directRoles = await _userRepository.GetUserDirectRolesAsync(request.UserId, cancellationToken);
        var directRoleDtos = directRoles.Select(r => new RoleDto(
            r.Id,
            r.Name,
            r.Description,
            r.RoleType.ToString())).ToList();

        List<RoleDto>? inheritedRoleDtos = null;

        if (request.IncludeInheritedRoles)
        {
            // Get effective roles (direct + inherited)
            var effectiveRoles = await _userRepository.GetUserEffectiveRolesAsync(request.UserId, cancellationToken);

            // Inherited roles = effective roles - direct roles
            var inheritedRoles = effectiveRoles
                .Where(er => !directRoles.Any(dr => dr.Id == er.Id))
                .ToList();

            inheritedRoleDtos = inheritedRoles.Select(r => new RoleDto(
                r.Id,
                r.Name,
                r.Description,
                r.RoleType.ToString())).ToList();
        }

        var response = new UserRolesResponse(
            request.UserId,
            directRoleDtos,
            inheritedRoleDtos);

        return Result.Success(response);
    }
}
