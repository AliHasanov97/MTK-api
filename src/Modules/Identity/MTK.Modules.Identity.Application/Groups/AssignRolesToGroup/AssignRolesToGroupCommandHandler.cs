using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.AssignRolesToGroup;

internal sealed class AssignRolesToGroupCommandHandler : ICommandHandler<AssignRolesToGroupCommand>
{
    private readonly IAuthenticationService _authenticationService;

    public AssignRolesToGroupCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(AssignRolesToGroupCommand request, CancellationToken cancellationToken)
    {
        await _authenticationService.AssignRolesToGroupAsync(
            request.GroupId,
            request.RoleNames,
            cancellationToken);

        return Result.Success();
    }
}
