using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Users;

/// <summary>
/// Identity-dəki User-in bu moduldakı snapshot-u (Id = Identity User.Id). Identity-nin
/// user integration event-ləri (Created/Updated/RoleChanged/Deleted) ilə sync edilir —
/// modullar arası DB join olmadığı üçün, bu modulun öz AuditLog-undakı UserId-ni
/// oxunan ad/email-ə çevirmək üçün istifadə olunur. Owner-dən fərqli olaraq HƏR
/// istifadəçini (admin, komandant, mühasib və s., təkcə sahibləri yox) əhatə edir.
/// IdentityId Keycloak subject id-sidir (Identity.User.IdentityId-in sync edilmiş
/// surəti) — Keycloak sync-in özü (status/retry) buraya daşınmır, yalnız dəyərin özü.
/// </summary>
public sealed class User : SearchableEntity
{
    private User(Guid id, string firstName, string lastName, string email, string? phoneNumber, string? identityId, UserStatus status) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        IdentityId = identityId;
        Status = status;
    }

    // Private constructor for EF Core
    private User() : base()
    {
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? IdentityId { get; private set; }
    public UserStatus Status { get; private set; }
    public string FullName => $"{FirstName} {LastName}";

    public static User Create(Guid id, string firstName, string lastName, string email, string? phoneNumber, string? identityId = null)
    {
        var user = new User(id, firstName, lastName, email, phoneNumber, identityId, UserStatus.Active);
        user.SetCreatedAt();
        return user;
    }

    public void Update(string firstName, string lastName, string email, string? phoneNumber, string? identityId = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        if (!string.IsNullOrWhiteSpace(identityId))
        {
            IdentityId = identityId;
        }
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        SetUpdatedAt();
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        SetUpdatedAt();
    }
}
