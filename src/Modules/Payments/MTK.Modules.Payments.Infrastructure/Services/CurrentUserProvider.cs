using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Payments.Application.Abstractions;

namespace MTK.Modules.Payments.Infrastructure.Services;

/// <summary>
/// The one place in Payments that's allowed to know Identity's concrete
/// <see cref="IUserContext"/> — Infrastructure already references
/// Identity.Application for this (see PaymentsDbContext's audit actor
/// resolution), so this just extends that existing, narrow coupling instead
/// of exposing it to Presentation too.
/// </summary>
internal sealed class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IUserContext _userContext;

    public CurrentUserProvider(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public Guid UserId => _userContext.UserId;
}
