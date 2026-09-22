using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Roles.CreateRealmRole;

internal sealed class CreateRealmRoleCommandHandler : ICommandHandler<CreateRealmRoleCommand, string>
{
    private readonly IAuthenticationService _authenticationService;

    public CreateRealmRoleCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<string>> Handle(CreateRealmRoleCommand request, CancellationToken cancellationToken)
    {
        // Create role directly in Keycloak
        await _authenticationService.CreateRealmRoleAsync(
            request.Name,
            request.Description ?? string.Empty,
            cancellationToken);

        // Return role name as identifier
        return Result.Success(request.Name);
    }
}
