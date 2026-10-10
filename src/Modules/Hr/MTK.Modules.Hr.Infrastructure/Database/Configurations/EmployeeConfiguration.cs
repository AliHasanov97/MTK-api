using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmployeeEducationHistories;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.RegisterNumber)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Surname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.FathersName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Nationality)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(e => e.Gender)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.BirthDate)
            .IsRequired(false);

        builder.Property(e => e.FinCode)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(e => e.IdCardNumber)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(e => e.SocialSecurityNumber)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(e => e.ContractNumber)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(e => e.SalaryBankName)
            .IsRequired(false)
            .HasMaxLength(200);

        builder.Property(e => e.EmployeeBankAccountNumber)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(e => e.MaritalStatus)
            .IsRequired(false)
            .HasConversion<int?>();

        builder.Property(e => e.NumberOfChildren)
            .IsRequired(false);

        builder.Property(e => e.MilitaryService)
            .IsRequired(false)
            .HasConversion<int?>();

        builder.Property(e => e.Veteran)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.Disability)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.Education)
            .IsRequired(false)
            .HasConversion<int?>();

        builder.Property(e => e.PhoneNumber)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(e => e.HomePhoneNumber)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(e => e.Email)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(e => e.RegisteredAddress)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(e => e.CurrentAddress)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(e => e.WorkingDays)
            .IsRequired(false)
            .HasConversion<int?>();

        builder.Property(e => e.VacationDays)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.ChildrenUnder14Count)
            .IsRequired(false);

        builder.Property(e => e.IsKarabakhWorker)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.IsSingleParent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.HasDisabledChild)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.StartWorkDate)
            .IsRequired();

        builder.Property(e => e.JobId)
            .IsRequired();

        builder.HasOne(e => e.Job)
            .WithMany()
            .HasForeignKey(e => e.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(EmployeeStatus.Active);

        builder.Property(e => e.EmploymentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(EmploymentType.FullTime);

        builder.Property(e => e.EmploymentOrderId)
            .IsRequired(false);

        builder.HasOne(e => e.EmploymentOrder)
            .WithOne()
            .HasForeignKey<Employee>(e => e.EmploymentOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EmploymentOrderId)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL AND \"EmploymentOrderId\" IS NOT NULL");

        builder.Property(e => e.CreatedById)
            .IsRequired();

        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.SearchVector)
            .HasColumnType("tsvector")
            .HasComputedColumnSql(
                "to_tsvector('simple', coalesce(\"Name\",'') || ' ' || coalesce(\"Surname\",'') || ' ' || coalesce(\"FathersName\",''))",
                stored: true);

        builder.HasIndex(e => e.SearchVector)
            .HasMethod("GIN");

        builder.HasIndex(e => e.CreatedAt);

        // ==================== CASCADE DELETE (İŞÇİ SİLİNƏNDƏ BÜTÜN DATALAR SİLİNSİN) ====================
        
        builder.HasMany(e => e.WorkHistories)
            .WithOne(wh => wh.Employee)
            .HasForeignKey(wh => wh.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.EducationHistories)
            .WithOne(eh => eh.Employee)
            .HasForeignKey(eh => eh.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.FileAttachments)
            .WithOne(fa => fa.Employee)
            .HasForeignKey(fa => fa.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        // =================================================================================================
    }
}