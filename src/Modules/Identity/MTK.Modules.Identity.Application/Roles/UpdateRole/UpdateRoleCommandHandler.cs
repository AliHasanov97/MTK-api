using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Application.Roles.UpdateRole;

internal sealed class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(
        IRoleRepository roleRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        Role? role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound(request.Id));
        }

        string oldName = role.Name;

        // Update in domain
        role.Update(request.Name, request.Description);

        if (request.IsActive && !role.IsActive)
        {
            role.Activate();
        }
        else if (!request.IsActive && role.IsActive)
        {
            role.Deactivate();
        }

        _roleRepository.Update(role);

        // Update in Keycloak (if name changed)
        if (oldName != request.Name)
        {
            await _authenticationService.UpdateRealmRoleAsync(
                oldName,
                request.Name,
                request.Description ?? string.Empty,
                cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
