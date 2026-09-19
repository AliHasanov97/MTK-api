using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Application.Roles.GetRoleById;

internal sealed class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, RoleDetailResponse>
{
    private readonly IRoleRepository _roleRepository;

    public GetRoleByIdQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<RoleDetailResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        Role? role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (role is null)
        {
            return Result.Failure<RoleDetailResponse>(RoleErrors.NotFound(request.Id));
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
