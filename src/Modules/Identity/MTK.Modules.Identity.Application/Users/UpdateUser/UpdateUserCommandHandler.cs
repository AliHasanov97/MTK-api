using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions.Data;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.UpdateUser;

internal sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.Id));
        }

        user.Update(
            request.FirstName,
            request.LastName,
            request.PhoneNumber);

        _userRepository.Update(user);

        // Update in Keycloak
        await _authenticationService.UpdateUserAsync(
            user.IdentityId,
            request.FirstName,
            request.LastName,
            user.Email,
            request.PhoneNumber);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
