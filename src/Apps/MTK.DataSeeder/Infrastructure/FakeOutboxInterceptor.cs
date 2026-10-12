using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MTK.DataSeeder.Infrastructure;

/// <summary>
/// Fake OutboxInterceptor for CLI/Seeder - outbox messages yazmır (lazım deyil)
/// CLI-də integration events lazım deyil, ona görə domain events-i ignore edirik
/// </summary>
internal sealed class FakeOutboxInterceptor : SaveChangesInterceptor
{
    // Heç bir şey override etmək lazım deyil - default behavior saxlanır
    // Domain events collect olunur amma outbox-a yazılmır
}
