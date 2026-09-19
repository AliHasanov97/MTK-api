using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Application.Roles.CreateRealmRole;

internal sealed class CreateRealmRoleCommandHandler : ICommandHandler<CreateRealmRoleCommand, Guid>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRealmRoleCommandHandler(
        IRoleRepository roleRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateRealmRoleCommand request, CancellationToken cancellationToken)
    {
        // Check if role already exists
        var existingRole = await _roleRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingRole != null)
        {
            return Result.Failure<Guid>(RoleErrors.AlreadyExists(request.Name));
        }

        // Create role in database
        var role = Role.Create(request.Name, request.RoleType, request.Description);
        _roleRepository.Add(role);

        // Create role in Keycloak
        await _authenticationService.CreateRealmRoleAsync(
            request.Name,
            request.Description ?? string.Empty,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(role.Id);
    }
}
