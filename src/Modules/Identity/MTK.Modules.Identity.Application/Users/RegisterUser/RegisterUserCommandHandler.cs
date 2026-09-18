using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Failure<Guid>(new Error("User.AlreadyExists", "User with this email already exists"));
        }

        // Create user in Keycloak
        string identityId;
        try
        {
            identityId = await _authenticationService.RegisterAsync(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password,
                cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(new Error("Keycloak.RegistrationFailed", $"Failed to register user in Keycloak: {ex.Message}"));
        }

        // Create user in local database
        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber);

        user.SetIdentityId(identityId);
        user.MarkSynced(); // Already synced since we just created in Keycloak

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
