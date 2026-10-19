using MTK.Modules.Communications.Application.Abstractions.Authentication;
using IdentityUserContext = MTK.Modules.Identity.Application.Abstractions.IUserContext;

namespace MTK.Modules.Communications.Infrastructure.Authentication;

internal sealed class UserContext(IdentityUserContext identityUserContext) : IUserContext
{
    public Guid UserId => identityUserContext.UserId;
}
