using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using MTK.Common.Application.Auditing;
using MTK.Common.Application.Authorization;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Warehouse.Application.Abstractions.Data;
using MTK.Modules.Warehouse.Domain.AuditLogs;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.Users;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Infrastructure.Database;

public sealed class WarehouseDbContext : DbContext, IUnitOfWork, IHasAuditActor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContext _userContext;
    private readonly IAuditActorAccessor _auditActorAccessor;

    public WarehouseDbContext(
        DbContextOptions<WarehouseDbContext> options,
        IHttpContextAccessor httpContextAccessor,
        IUserContext userContext,
        IAuditActorAccessor auditActorAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _userContext = userContext;
        _auditActorAccessor = auditActorAccessor;
    }

    public Guid? CurrentActorUserId { get; private set; }

    public DbSet<Nomenclature> Nomenclatures => Set<Nomenclature>();

    public DbSet<WarehouseTransaction> WarehouseTransactions => Set<WarehouseTransaction>();

    public DbSet<WarehouseStock> WarehouseStocks => Set<WarehouseStock>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Identity-dən sync edilən User snapshot-u (AuditLog.UserId -> ad/email üçün)
    public DbSet<User> Users => Set<User>();

    // Outbox Pattern
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers => Set<OutboxMessageConsumer>();

    // Inbox Pattern
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<InboxMessageConsumer> InboxMessageConsumers => Set<InboxMessageConsumer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("warehouse");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);

        // Apply Outbox and Inbox configurations from Common.Infrastructure
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        ConvertDeletesToSoftDeletes();

        var httpContextUser = _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true
            ? _httpContextAccessor.HttpContext.User
            : null;

        Guid? actorUserId = _auditActorAccessor.ActorUserId
            ?? (httpContextUser is not null ? _userContext.UserId : null);
        CurrentActorUserId = actorUserId;

        // Only resolvable live, from the request's own JWT claims — a background
        // (outbox-driven) SaveChanges has no HttpContext and so no role either.
        string? actorRole = httpContextUser is not null ? ResolveActorRole(httpContextUser) : null;

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
                    : JsonSerializer.Serialize(entry.Properties.ToDictionary(property => property.Metadata.Name, property => property.OriginalValue));
                var newValues = entry.State == EntityState.Deleted
                    ? null
                    : JsonSerializer.Serialize(entry.Properties.ToDictionary(property => property.Metadata.Name, property => property.CurrentValue));

                return AuditLog.Create(entry.Metadata.ClrType.Name, entry.Entity.Id, action, oldValues, newValues, actorUserId, actorRole);
            })
            .ToList();

        AuditLogs.AddRange(auditEntries);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ConvertDeletesToSoftDeletes()
    {
        foreach (var entry in ChangeTracker.Entries<Entity>().Where(entry => entry.State == EntityState.Deleted))
        {
            var deletedAt = entry.Properties.FirstOrDefault(property => property.Metadata.Name == "DeletedAt");
            if (deletedAt is null)
                continue;

            // DeletedAt DateTimeOffset?-dır — DateTime yazılsa, EF dəyəri geri
            // oxuyarkən InvalidCastException verir (soft delete tamamilə sınırdı).
            deletedAt.CurrentValue = DateTimeOffset.UtcNow;
            deletedAt.IsModified = true;
            entry.State = EntityState.Modified;
        }
    }

    private static bool IsSoftDelete(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Entity> entry)
    {
        var deletedAt = entry.Properties.FirstOrDefault(property => property.Metadata.Name == "DeletedAt");
        return deletedAt?.IsModified == true && deletedAt.OriginalValue is null && deletedAt.CurrentValue is not null;
    }

    // A Keycloak token typically carries several realm roles at once (the business
    // one alongside "default-roles-mtk", "offline_access", etc.) — pick the most
    // specific/operational one a user could actually be acting as here, in priority
    // order. Admin is checked last since it's a superset, not a distinct capacity.
    private static readonly string[] RolePriority =
    [
        Roles.BuildingManager,
        Roles.Accountant,
        Roles.Owner,
        Roles.Employee,
        Roles.Admin,
    ];

    private static string? ResolveActorRole(ClaimsPrincipal user)
    {
        foreach (var role in RolePriority)
        {
            if (user.IsInRole(role))
            {
                return role;
            }
        }

        return null;
    }
}
