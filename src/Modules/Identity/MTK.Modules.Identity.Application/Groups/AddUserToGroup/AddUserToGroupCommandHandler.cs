using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Groups.AddUserToGroup;

internal sealed class AddUserToGroupCommandHandler : ICommandHandler<AddUserToGroupCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;

    public AddUserToGroupCommandHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(AddUserToGroupCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        await _authenticationService.AddUserToGroupAsync(
            user.IdentityId,
            request.GroupId,
            cancellationToken);

        return Result.Success();
    }
}
