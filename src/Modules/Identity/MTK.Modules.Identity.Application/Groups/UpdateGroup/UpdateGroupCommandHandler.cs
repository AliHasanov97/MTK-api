using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.UpdateGroup;

internal sealed class UpdateGroupCommandHandler : ICommandHandler<UpdateGroupCommand>
{
    private readonly IAuthenticationService _authenticationService;

    public UpdateGroupCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        await _authenticationService.UpdateGroupAsync(
            request.Id,
            request.Name,
            request.Description,
            cancellationToken);

        return Result.Success();
    }
}
