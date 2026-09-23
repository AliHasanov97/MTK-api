using MTK.Common.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Application.Abstractions.Data;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting user registration for email: {Email}", request.Email);

        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            _logger.LogWarning("User with email {Email} already exists in database", request.Email);
            return Result.Failure<Guid>(new Error("User.AlreadyExists", "User with this email already exists"));
        }

        // Create user in Keycloak
        string identityId;
        try
        {
            _logger.LogInformation("Creating user in Keycloak: {Email}", request.Email);
            identityId = await _authenticationService.RegisterAsync(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password,
                cancellationToken);
            _logger.LogInformation("User created in Keycloak successfully. IdentityId: {IdentityId}, Email: {Email}", identityId, request.Email);

            // Assign roles if provided
            if (request.RoleNames?.Length > 0)
            {
                _logger.LogInformation("Assigning roles to user {IdentityId}: {Roles}", identityId, string.Join(", ", request.RoleNames));
                await _authenticationService.AssignRealmRolesToUserAsync(
                    identityId,
                    request.RoleNames,
                    cancellationToken);
                _logger.LogInformation("Roles assigned successfully to user {IdentityId}", identityId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user in Keycloak for email: {Email}", request.Email);
            return Result.Failure<Guid>(new Error("Keycloak.RegistrationFailed", $"Failed to register user in Keycloak: {ex.Message}"));
        }

        // Create user in local database
        try
        {
            _logger.LogInformation("Creating user entity in database for email: {Email}", request.Email);
            var user = User.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                UserStatus.Active,
                request.RoleNames);

            user.SetIdentityId(identityId);
            user.MarkSynced();

            _userRepository.Add(user);

            _logger.LogInformation("Calling SaveChangesAsync for user: {Email}", request.Email);

            _logger.LogInformation("DEBUG: _unitOfWork type = {Type}, HashCode = {HashCode}",
                _unitOfWork.GetType().FullName,
                _unitOfWork.GetHashCode());

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("SaveChangesAsync returned {Result} rows affected", result);

            _logger.LogInformation("User successfully registered. UserId: {UserId}, Email: {Email}, IdentityId: {IdentityId}", user.Id, request.Email, identityId);
            return Result.Success(user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save user to database. Email: {Email}, IdentityId: {IdentityId}", request.Email, identityId);

            // Try to cleanup Keycloak user since DB save failed
            try
            {
                _logger.LogWarning("Attempting to cleanup Keycloak user after DB failure: {IdentityId}", identityId);
                await _authenticationService.DeleteAsync(identityId, cancellationToken);
                _logger.LogInformation("Successfully cleaned up Keycloak user: {IdentityId}", identityId);
            }
            catch (Exception cleanupEx)
            {
                _logger.LogError(cleanupEx, "Failed to cleanup Keycloak user: {IdentityId}. Manual cleanup required!", identityId);
            }

            return Result.Failure<Guid>(new Error("User.RegistrationFailed", $"Failed to save user to database: {ex.Message}"));
        }
    }
}
