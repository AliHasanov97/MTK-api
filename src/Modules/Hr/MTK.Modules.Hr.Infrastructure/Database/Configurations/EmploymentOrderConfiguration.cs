using MTK.Modules.Hr.Domain.EmploymentOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class EmploymentOrderConfiguration : IEntityTypeConfiguration<EmploymentOrder>
{
    public void Configure(EntityTypeBuilder<EmploymentOrder> builder)
    {
        builder.ToTable("employment_orders");

        builder.Property(e => e.JobApplicationId)
            .IsRequired();

        builder.HasOne(e => e.JobApplication)
            .WithOne(ja => ja.EmploymentOrder)
            .HasForeignKey<EmploymentOrder>(e => e.JobApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.JobApplicationId)
            .IsUnique();

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired();

        builder.Property(e => e.LaborCodeCaseId)
            .IsRequired();

        builder.HasOne(e => e.LaborCodeCase)
            .WithMany()
            .HasForeignKey(e => e.LaborCodeCaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.LaborCodeCaseId);
    }
}
