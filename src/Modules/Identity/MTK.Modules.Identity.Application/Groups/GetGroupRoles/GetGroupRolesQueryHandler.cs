using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Groups;

namespace MTK.Modules.Identity.Application.Groups.GetGroupRoles;

internal sealed class GetGroupRolesQueryHandler : IQueryHandler<GetGroupRolesQuery, List<RoleDto>>
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupRolesQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<Result<List<RoleDto>>> Handle(GetGroupRolesQuery request, CancellationToken cancellationToken)
    {
        Group? group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            return Result.Failure<List<RoleDto>>(GroupErrors.NotFound(request.GroupId));
        }

        var roles = await _groupRepository.GetGroupRolesAsync(request.GroupId, cancellationToken);

        var roleDtos = roles.Select(r => new RoleDto(
            r.Id,
            r.Name,
            r.Description,
            r.RoleType.ToString())).ToList();

        return Result.Success(roleDtos);
    }
}
