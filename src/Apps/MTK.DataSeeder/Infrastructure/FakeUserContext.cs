using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.DataSeeder.Infrastructure;

/// <summary>
/// Fake UserContext for CLI/Seeder - BuildingsDbContext audit fields üçün lazımdır
/// </summary>
internal sealed class FakeUserContext : IUserContext
{
    public Guid UserId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001"); // System user
    public string IdentityId { get; } = "system";
    public string FirstName { get; } = "System";
    public string LastName { get; } = "User";
    public string Email { get; } = "system@mtk.local";
    public string? PhoneNumber { get; } = null;
    public string Role { get; } = "SystemAdmin";
}
