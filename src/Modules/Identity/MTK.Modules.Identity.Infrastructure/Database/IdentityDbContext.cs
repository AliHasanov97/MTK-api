using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using IUnitOfWork = MTK.Modules.Identity.Application.Abstractions.Data.IUnitOfWork;
using MTK.Modules.Identity.Domain.Users;
using MTK.Modules.Identity.Domain.AuditLogs;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Infrastructure.Database;

public sealed class IdentityDbContext : DbContext, IUnitOfWork
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
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
        ChangeTracker.DetectChanges();

        var principal = _httpContextAccessor.HttpContext?.User;
        var identityId = principal?.Identity?.IsAuthenticated == true
            ? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            : null;
        Guid? actorUserId = identityId is null
            ? null
            : await Users.IgnoreQueryFilters()
                .AsNoTracking()
                .Where(user => user.IdentityId == identityId)
                .Select(user => (Guid?)user.Id)
                .SingleOrDefaultAsync(cancellationToken);

        var auditEntries = ChangeTracker.Entries<Entity>()
            .Where(entry => entry.Entity is not AuditLog)
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(entry =>
            {
                var action = entry.State switch
                {
                    EntityState.Added => "Created",
                    EntityState.Deleted => "Deleted",
                    _ when IsSoftDelete(entry) => "Deleted",
                    _ => "Updated"
                };

                var oldValues = entry.State == EntityState.Added
                    ? null
                    : JsonSerializer.Serialize(entry.Properties.ToDictionary(
                        property => property.Metadata.Name,
                        property => property.OriginalValue));
                var newValues = entry.State == EntityState.Deleted
                    ? null
                    : JsonSerializer.Serialize(entry.Properties.ToDictionary(
                        property => property.Metadata.Name,
                        property => property.CurrentValue));

                return AuditLog.Create(entry.Metadata.ClrType.Name, entry.Entity.Id, action, oldValues, newValues, actorUserId);
            })
            .ToList();

        AuditLogs.AddRange(auditEntries);

        // Domain events are automatically captured by InsertOutboxMessagesInterceptor
        // ProcessOutboxJob will handle publishing them asynchronously
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    private static bool IsSoftDelete(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Entity> entry)
    {
        var deletedAt = entry.Properties.FirstOrDefault(property => property.Metadata.Name == "DeletedAt");
        return deletedAt?.IsModified == true && deletedAt.OriginalValue is null && deletedAt.CurrentValue is not null;
    }
}
