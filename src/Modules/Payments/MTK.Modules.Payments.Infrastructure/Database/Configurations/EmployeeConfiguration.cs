using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Employees;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.BaseSalary)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Mövcud sətirlər (bu sütun əlavə olunmazdan əvvəl sync olunmuş) aktiv sayılır.
        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
