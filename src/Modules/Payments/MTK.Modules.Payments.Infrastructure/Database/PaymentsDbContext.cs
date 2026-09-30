using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using IUnitOfWork = MTK.Modules.Payments.Application.Abstractions.Data.IUnitOfWork;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.OwnerBalances;
using MTK.Modules.Payments.Domain.AuditLogs;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Transactions;
using MTK.Modules.Payments.Domain.Vendors;
using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Infrastructure.Database;

public sealed class PaymentsDbContext : DbContext, IUnitOfWork
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContext _userContext;

    public PaymentsDbContext(
        DbContextOptions<PaymentsDbContext> options,
        IHttpContextAccessor httpContextAccessor,
        IUserContext userContext)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _userContext = userContext;
    }

    public DbSet<Rate> Rates { get; set; }
    public DbSet<Charge> Charges { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentAllocation> PaymentAllocations { get; set; }
    public DbSet<OwnerBalance> OwnerBalances { get; set; }
    public DbSet<PropertyOwnership> PropertyOwnerships { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractService> ContractServices { get; set; }
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

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

        Guid? actorUserId = _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true
            ? _userContext.UserId
            : null;

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

                return AuditLog.Create(entry.Metadata.ClrType.Name, entry.Entity.Id, action, oldValues, newValues, actorUserId);
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
}
