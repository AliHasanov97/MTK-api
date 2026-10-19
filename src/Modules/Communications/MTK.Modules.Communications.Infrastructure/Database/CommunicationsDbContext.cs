using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using MTK.Common.Application.Auditing;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using MTK.Modules.Communications.Application.Abstractions.Authentication;
using MTK.Modules.Communications.Application.Abstractions.Data;
using MTK.Modules.Communications.Domain.Articles;
using MTK.Modules.Communications.Domain.FileAttachments;
using MTK.Modules.Communications.Domain.Users;

namespace MTK.Modules.Communications.Infrastructure.Database;

public sealed class CommunicationsDbContext : DbContext, IUnitOfWork, IHasAuditActor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContext _userContext;
    private readonly IAuditActorAccessor _auditActorAccessor;

    public CommunicationsDbContext(
        DbContextOptions<CommunicationsDbContext> options,
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

    public DbSet<Article> Articles => Set<Article>();

    public DbSet<FileAttachment> FileAttachments => Set<FileAttachment>();

    // Identity-dən sync edilən User snapshot-u (CreatedById -> ad/email üçün)
    public DbSet<User> Users => Set<User>();

    // Outbox Pattern
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers => Set<OutboxMessageConsumer>();

    // Inbox Pattern
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<InboxMessageConsumer> InboxMessageConsumers => Set<InboxMessageConsumer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("communications");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommunicationsDbContext).Assembly);

        // Apply Outbox and Inbox configurations from Common.Infrastructure
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());

        base.OnModelCreating(modelBuilder);

        ApplySoftDeleteFilters(modelBuilder);
    }

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

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void StampTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
            {
                entry.Entity.SetCreatedAt();
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.SetUpdatedAt();
            }
        }
    }
}
