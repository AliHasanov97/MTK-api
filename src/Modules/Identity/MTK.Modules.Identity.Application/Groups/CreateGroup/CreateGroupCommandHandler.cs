using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.CreateGroup;

internal sealed class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand, Guid>
{
    private readonly IAuthenticationService _authenticationService;

    public CreateGroupCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<Guid>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        Guid keycloakGroupId = await _authenticationService.CreateGroupAsync(
            request.Name,
            request.Description,
            cancellationToken);

        return Result.Success(keycloakGroupId);
    }
}
