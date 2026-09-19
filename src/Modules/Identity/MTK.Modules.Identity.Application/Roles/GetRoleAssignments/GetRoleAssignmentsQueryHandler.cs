using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Application.Roles.GetRoleAssignments;

internal sealed class GetRoleAssignmentsQueryHandler : IQueryHandler<GetRoleAssignmentsQuery, RoleAssignmentsResponse>
{
    private readonly IRoleRepository _roleRepository;

    public GetRoleAssignmentsQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<RoleAssignmentsResponse>> Handle(GetRoleAssignmentsQuery request, CancellationToken cancellationToken)
    {
        Role? role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (role is null)
        {
            return Result.Failure<RoleAssignmentsResponse>(RoleErrors.NotFound(request.Id));
        }

        // Get groups with this role
        var groups = await _roleRepository.GetGroupsWithRoleAsync(request.Id, cancellationToken);
        var groupInfos = groups.Select(g => new GroupInfo(g.Id, g.Name)).ToList();

        // Get users with direct role assignment
        var users = await _roleRepository.GetUsersWithDirectRoleAsync(request.Id, cancellationToken);
        int userCount = users.Count;

        var response = new RoleAssignmentsResponse(
            role.Id,
            role.Name,
            groupInfos,
            userCount);

        return Result.Success(response);
    }
}
