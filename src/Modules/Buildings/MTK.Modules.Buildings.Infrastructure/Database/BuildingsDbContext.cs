using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using IUnitOfWork = MTK.Modules.Buildings.Application.Abstractions.Data.IUnitOfWork;
using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Buildings;
using MTK.Modules.Buildings.Domain.Garages;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.OwnershipHistories;

namespace MTK.Modules.Buildings.Infrastructure.Database;

public sealed class BuildingsDbContext : DbContext, IUnitOfWork
{
    public BuildingsDbContext(DbContextOptions<BuildingsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Building> Buildings { get; set; }
    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Garage> Garages { get; set; }
    public DbSet<OwnershipHistory> OwnershipHistories { get; set; }

    // Outbox Pattern
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers => Set<OutboxMessageConsumer>();

    // Inbox Pattern
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<InboxMessageConsumer> InboxMessageConsumers => Set<InboxMessageConsumer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("buildings");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BuildingsDbContext).Assembly);

        // Apply Outbox and Inbox configurations from Common.Infrastructure
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
