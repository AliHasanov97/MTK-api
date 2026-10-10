using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using MTK.Common.Application.Auditing;
using MTK.Common.Application.Authorization;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.AuditLogs;
using MTK.Modules.Hr.Domain.Users;

namespace MTK.Modules.Hr.Infrastructure.Database;

public sealed class HrDbContext : DbContext, IUnitOfWork, IHasAuditActor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContext _userContext;
    private readonly IAuditActorAccessor _auditActorAccessor;

    public HrDbContext(
        DbContextOptions<HrDbContext> options,
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
        modelBuilder.HasDefaultSchema("hr");

        // Ərizə/əmr nömrələri bazanın ardıcıllıqlarından verilir (fivestar-dakı kimi).
        modelBuilder.HasSequence<int>("application_number_seq", "hr").StartsAt(1).IncrementsBy(1);
        modelBuilder.HasSequence<int>("order_number_seq", "hr").StartsAt(1).IncrementsBy(1);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrDbContext).Assembly);

        // Apply Outbox and Inbox configurations from Common.Infrastructure
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());

        base.OnModelCreating(modelBuilder);

        ApplySoftDeleteFilters(modelBuilder);
    }

    // Npgsql "timestamp with time zone" yalnız UTC offset (0) qəbul edir; API-dən gələn tarixlər fərqli offset daşıya bilər.
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<UtcDateTimeOffsetConverter>();
        configurationBuilder.Properties<DateTimeOffset?>().HaveConversion<NullableUtcDateTimeOffsetConverter>();
    }

    private sealed class UtcDateTimeOffsetConverter()
        : ValueConverter<DateTimeOffset, DateTimeOffset>(v => new DateTimeOffset(v.UtcDateTime, TimeSpan.Zero), v => v);

    private sealed class NullableUtcDateTimeOffsetConverter()
        : ValueConverter<DateTimeOffset?, DateTimeOffset?>(
            v => v.HasValue ? new DateTimeOffset(v.Value.UtcDateTime, TimeSpan.Zero) : null, v => v);

    /// <summary>Hər kök entity üçün DeletedAt == null filtri (öz filtri olanlara toxunulmur).</summary>
    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.BaseType is not null
                || !typeof(Entity).IsAssignableFrom(entityType.ClrType)
                || entityType.GetQueryFilter() is not null)
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var deletedAt = Expression.Property(parameter, nameof(Entity.DeletedAt));
            var filter = Expression.Lambda(Expression.Equal(deletedAt, Expression.Constant(null, typeof(DateTimeOffset?))), parameter);
            entityType.SetQueryFilter(filter);
        }
    }

    public Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => BeginWrappedAsync(cancellationToken);

    private async Task<ITransaction> BeginWrappedAsync(CancellationToken cancellationToken)
        => await MTK.Common.Infrastructure.Database.TransactionWrapper.BeginTransactionAsync(this, cancellationToken);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        StampTimestamps();

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

    // fivestar-da bunu AuditableInterceptor edir: yeni qeydə CreatedAt, dəyişənə UpdatedAt qoyulur.
    private void StampTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
            {
                entry.Entity.SetCreatedAt();
            }
            else if (entry.State == EntityState.Modified && entry.Entity is not AuditLog)
            {
                entry.Entity.SetUpdatedAt();
            }
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
