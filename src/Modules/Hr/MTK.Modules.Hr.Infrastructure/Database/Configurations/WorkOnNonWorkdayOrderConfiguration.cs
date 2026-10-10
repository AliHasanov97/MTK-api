using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class WorkOnNonWorkdayOrderConfiguration : IEntityTypeConfiguration<WorkOnNonWorkdayOrder>
{
    public void Configure(EntityTypeBuilder<WorkOnNonWorkdayOrder> builder)
    {
        builder.ToTable("work_on_non_workday_orders");

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired(false);
    }
}
