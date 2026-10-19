using MTK.Modules.Buildings.Application.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Buildings.Infrastructure.Services;

/// <summary>
/// Hazırki istifadəçinin ID-si = Identity User.Id (bu modulun User snapshot-unun Id-si ilə eyni).
/// JWT-dəki NameIdentifier claim-i isə Keycloak subject-idir (IdentityId), User.Id deyil —
/// onu birbaşa oxumaq müraciətin SubmitterId-sini səhv ID ilə yazır və ad axtarışı null qaytarır.
/// IUserContext Keycloak subject-i IdentityId üzrə Identity User.Id-yə çevirir.
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
