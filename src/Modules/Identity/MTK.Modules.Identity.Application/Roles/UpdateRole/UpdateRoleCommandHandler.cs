using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Roles.UpdateRole;

internal sealed class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand>
{
    private readonly IAuthenticationService _authenticationService;

    public UpdateRoleCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        // Update role directly in Keycloak
        await _authenticationService.UpdateRealmRoleAsync(
            request.OldRoleName,
            request.NewRoleName,
            request.Description ?? string.Empty,
            cancellationToken);

        return Result.Success();
    }
}
