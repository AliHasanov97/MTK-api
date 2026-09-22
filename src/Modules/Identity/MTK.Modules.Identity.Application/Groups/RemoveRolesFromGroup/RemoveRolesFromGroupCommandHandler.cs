using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.RemoveRolesFromGroup;

internal sealed class RemoveRolesFromGroupCommandHandler : ICommandHandler<RemoveRolesFromGroupCommand>
{
    private readonly IAuthenticationService _authenticationService;

    public RemoveRolesFromGroupCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(RemoveRolesFromGroupCommand request, CancellationToken cancellationToken)
    {
        await _authenticationService.RemoveRolesFromGroupAsync(
            request.GroupId,
            request.RoleNames,
            cancellationToken);

        return Result.Success();
    }
}
