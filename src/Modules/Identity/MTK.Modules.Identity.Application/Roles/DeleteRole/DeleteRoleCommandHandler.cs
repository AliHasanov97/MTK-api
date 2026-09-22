using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Roles.DeleteRole;

internal sealed class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand>
{
    private readonly IAuthenticationService _authenticationService;

    public DeleteRoleCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        // Delete role directly from Keycloak
        await _authenticationService.DeleteRealmRoleAsync(request.RoleName, cancellationToken);

        return Result.Success();
    }
}
