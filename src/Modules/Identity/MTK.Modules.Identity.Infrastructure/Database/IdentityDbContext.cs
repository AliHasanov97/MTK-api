using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using IUnitOfWork = MTK.Modules.Identity.Application.Abstractions.Data.IUnitOfWork;
using MTK.Modules.Identity.Domain.Users;
using MTK.Modules.Identity.Domain.AuditLogs;

namespace MTK.Modules.Identity.Infrastructure.Database;

public sealed class IdentityDbContext : DbContext, IUnitOfWork
{
    private readonly ILogger<IdentityDbContext> _logger;

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        ILogger<IdentityDbContext> logger)
        : base(options)
    {
        _logger = logger;
        _logger.LogInformation("IdentityDbContext instance created. HashCode = {HashCode}", GetHashCode());
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Outbox Pattern
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers => Set<OutboxMessageConsumer>();

    // Inbox Pattern
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<InboxMessageConsumer> InboxMessageConsumers => Set<InboxMessageConsumer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        // Apply Outbox and Inbox configurations from Common.Infrastructure
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(u => u.DeletedAt == null);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("=== IdentityDbContext.SaveChangesAsync called === Instance HashCode = {HashCode}", GetHashCode());

        var entries = ChangeTracker.Entries().ToList();
        _logger.LogInformation("ChangeTracker has {Count} entries", entries.Count);

        foreach (var entry in entries)
        {
            _logger.LogInformation("Entity: {EntityType}, State: {State}, Keys: {Keys}",
                entry.Entity.GetType().Name,
                entry.State,
                string.Join(", ", entry.Properties.Where(p => p.Metadata.IsKey()).Select(p => $"{p.Metadata.Name}={p.CurrentValue}")));
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("SaveChangesAsync completed. Rows affected: {RowsAffected}", result);

        return result;
    }
}
