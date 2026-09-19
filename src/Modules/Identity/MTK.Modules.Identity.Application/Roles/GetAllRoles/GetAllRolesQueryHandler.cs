using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Application.Roles.GetAllRoles;

internal sealed class GetAllRolesQueryHandler : IQueryHandler<GetAllRolesQuery, List<RoleResponse>>
{
    private readonly IRoleRepository _roleRepository;

    public GetAllRolesQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<List<RoleResponse>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);

        var response = roles.Select(r => new RoleResponse(
            r.Id,
            r.Name,
            r.Description,
            r.RoleType.ToString(),
            r.IsActive)).ToList();

        return Result.Success(response);
    }
}
