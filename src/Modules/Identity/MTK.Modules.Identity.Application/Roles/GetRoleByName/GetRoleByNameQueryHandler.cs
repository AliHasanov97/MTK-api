using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Roles.GetRoleById;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Application.Roles.GetRoleByName;

internal sealed class GetRoleByNameQueryHandler : IQueryHandler<GetRoleByNameQuery, RoleDetailResponse>
{
    private readonly IRoleRepository _roleRepository;

    public GetRoleByNameQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<RoleDetailResponse>> Handle(GetRoleByNameQuery request, CancellationToken cancellationToken)
    {
        Role? role = await _roleRepository.GetByNameAsync(request.Name, cancellationToken);

        if (role is null)
        {
            return Result.Failure<RoleDetailResponse>(RoleErrors.NotFoundByName(request.Name));
        }

        var response = new RoleDetailResponse(
            role.Id,
            role.Name,
            role.Description,
            role.RoleType.ToString(),
            role.IsActive,
            role.CreatedAt,
            role.UpdatedAt);

        return Result.Success(response);
    }
}
