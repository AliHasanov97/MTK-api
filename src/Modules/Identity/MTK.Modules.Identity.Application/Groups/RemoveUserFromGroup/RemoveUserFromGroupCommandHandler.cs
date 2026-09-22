using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Groups.RemoveUserFromGroup;

internal sealed class RemoveUserFromGroupCommandHandler : ICommandHandler<RemoveUserFromGroupCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;

    public RemoveUserFromGroupCommandHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(RemoveUserFromGroupCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        await _authenticationService.RemoveUserFromGroupAsync(
            user.IdentityId,
            request.GroupId,
            cancellationToken);

        return Result.Success();
    }
}
