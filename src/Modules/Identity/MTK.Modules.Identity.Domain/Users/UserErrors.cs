using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) =>
        new Error("Users.NotFound", $"The user with Id '{userId}' was not found");

    public static Error NotFoundByEmail(string email) =>
        new Error("Users.NotFoundByEmail", $"The user with email '{email}' was not found");

    public static Error AlreadyExists(string email) =>
        new Error("Users.AlreadyExists", $"A user with email '{email}' already exists");

    public static Error CannotDelete(Guid userId) =>
        new Error("Users.CannotDelete", $"The user with Id '{userId}' cannot be deleted");
}
