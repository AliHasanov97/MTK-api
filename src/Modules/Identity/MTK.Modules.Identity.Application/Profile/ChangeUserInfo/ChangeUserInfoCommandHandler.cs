using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Profile.ChangeUserInfo;

internal sealed class ChangeUserInfoCommandHandler : ICommandHandler<ChangeUserInfoCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeUserInfoCommandHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeUserInfoCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(_userContext.UserId));
        }

        // Update phone number if provided
        if (request.PhoneNumber != null)
        {
            user.Update(user.FirstName, user.LastName, request.PhoneNumber);
            _userRepository.Update(user);
        }

        // Update password if provided
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            await _authenticationService.UpdateUserPasswordAsync(
                user.IdentityId ?? throw new InvalidOperationException("User identity ID is missing"),
                request.Password,
                cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
