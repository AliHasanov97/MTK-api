using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Infrastructure.Database;
using System.Security.Claims;

namespace MTK.Modules.Identity.Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IdentityDbContext _dbContext;
    private bool _isLoaded;

    private Guid _userId;
    private string _identityId = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private string? _phoneNumber;
    private string _role = string.Empty;

    public UserContext(IHttpContextAccessor httpContextAccessor, IdentityDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public Guid UserId
    {
        get
        {
            EnsureLoaded();
            return _userId;
        }
    }

    public string IdentityId
    {
        get
        {
            EnsureLoaded();
            return _identityId;
        }
    }

    public string FirstName
    {
        get
        {
            EnsureLoaded();
            return _firstName;
        }
    }

    public string LastName
    {
        get
        {
            EnsureLoaded();
            return _lastName;
        }
    }

    public string Email
    {
        get
        {
            EnsureLoaded();
            return _email;
        }
    }

    public string? PhoneNumber
    {
        get
        {
            EnsureLoaded();
            return _phoneNumber;
        }
    }

    public string Role
    {
        get
        {
            EnsureLoaded();
            return _role;
        }
    }

    private void EnsureLoaded()
    {
        if (_isLoaded)
        {
            return;
        }

        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity!.IsAuthenticated)
        {
            throw new InvalidOperationException("User is not authenticated");
        }

        // Get identity ID from claims
        _identityId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                     throw new InvalidOperationException("Identity ID not found in claims");

        // Try to get user ID from claims (if available)
        var userIdClaim = user.FindFirst("sub")?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            _userId = userId;
        }

        // Load user data from database
        var dbUser = _dbContext.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.IdentityId == _identityId);

        if (dbUser != null)
        {
            _userId = dbUser.Id;
            _firstName = dbUser.FirstName;
            _lastName = dbUser.LastName;
            _email = dbUser.Email;
            _phoneNumber = dbUser.PhoneNumber;
            // Role is now managed in Keycloak, get from claims
            _role = user.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        }
        else
        {
            // Fallback to claims if user not in database
            _email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            _firstName = user.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
            _lastName = user.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;
            _role = user.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        }

        _isLoaded = true;
    }
}
