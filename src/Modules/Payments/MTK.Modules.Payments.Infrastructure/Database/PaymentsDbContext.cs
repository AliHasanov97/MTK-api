using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using MTK.Common.Application.Auditing;
using MTK.Common.Application.Authorization;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using IUnitOfWork = MTK.Modules.Payments.Application.Abstractions.Data.IUnitOfWork;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.OwnerBalances;
using MTK.Modules.Payments.Domain.CompanyBalances;
using MTK.Modules.Payments.Domain.AuditLogs;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Transactions;
using MTK.Modules.Payments.Domain.Vendors;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Users;
using MTK.Modules.Payments.Domain.Buildings;
using MTK.Modules.Payments.Domain.Apartments;
using MTK.Modules.Payments.Domain.Garages;
using MTK.Modules.Payments.Domain.Owners;
using MTK.Modules.Payments.Domain.FileAttachments;

namespace MTK.Modules.Payments.Infrastructure.Database;

public sealed class PaymentsDbContext : DbContext, IUnitOfWork, IHasAuditActor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContext _userContext;
    private readonly IAuditActorAccessor _auditActorAccessor;

    public PaymentsDbContext(
        DbContextOptions<PaymentsDbContext> options,
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

    public DbSet<Rate> Rates { get; set; }
    public DbSet<Charge> Charges { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentAllocation> PaymentAllocations { get; set; }
    public DbSet<OwnerBalance> OwnerBalances { get; set; }
    public DbSet<CompanyBalance> CompanyBalances { get; set; }
    public DbSet<PropertyOwnership> PropertyOwnerships { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractService> ContractServices { get; set; }
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<Garage> Garages => Set<Garage>();
    public DbSet<FileAttachment> FileAttachments => Set<FileAttachment>();

    // Outbox Pattern
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers => Set<OutboxMessageConsumer>();

    // Inbox Pattern
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<InboxMessageConsumer> InboxMessageConsumers => Set<InboxMessageConsumer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly);

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
