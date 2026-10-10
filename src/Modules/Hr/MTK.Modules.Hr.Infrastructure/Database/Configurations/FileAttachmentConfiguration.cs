using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Domain.BonusOrders;
using MTK.Modules.Hr.Domain.CompensationOrders;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmploymentOrders;
using MTK.Modules.Hr.Domain.FileAttachments;
using MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;
using MTK.Modules.Hr.Domain.VacationApplications;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;
using MTK.Modules.Hr.Domain.VacationOrders;
using MTK.Modules.Hr.Domain.SalaryDeductions;
using MTK.Modules.Hr.Domain.Warnings;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using MTK.Modules.Hr.Domain.VacationReturnOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class FileAttachmentConfiguration : IEntityTypeConfiguration<FileAttachment>
{
    public void Configure(EntityTypeBuilder<FileAttachment> builder)
    {
        builder.ToTable("file_attachments");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.MimeType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.IsPublic)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(f => f.DocumentType)
            .IsRequired(false);

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.JobApplicationId)
            .IsRequired(false);

        builder.HasOne(f => f.JobApplication)
            .WithMany(ja => ja.FileAttachments)
            .HasForeignKey(f => f.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.EmploymentOrderId)
            .IsRequired(false);

        builder.HasOne(f => f.EmploymentOrder)
            .WithMany(eo => eo.FileAttachments)
            .HasForeignKey(f => f.EmploymentOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.EmployeeId)
            .IsRequired(false);

        builder.HasOne(f => f.Employee)
            .WithMany(e => e.FileAttachments)
            .HasForeignKey(f => f.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.ApplicationForChangeOfPositionId)
            .IsRequired(false);

        builder.HasOne(f => f.ApplicationForChangeOfPosition)
            .WithMany(a => a.FileAttachments)
            .HasForeignKey(f => f.ApplicationForChangeOfPositionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.OrderForChangeOfPositionId)
            .IsRequired(false);

        builder.HasOne(f => f.OrderForChangeOfPosition)
            .WithMany(o => o.FileAttachments)
            .HasForeignKey(f => f.OrderForChangeOfPositionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.UnexcusedAbsenceId)
            .IsRequired(false);

        builder.HasOne(f => f.UnexcusedAbsence)
            .WithMany(u => u.FileAttachments)
            .HasForeignKey(f => f.UnexcusedAbsenceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.NoticeOfChangeInWorkingConditionsId)
            .IsRequired(false);

        builder.HasOne(f => f.NoticeOfChangeInWorkingConditions)
            .WithMany(n => n.FileAttachments)
            .HasForeignKey(f => f.NoticeOfChangeInWorkingConditionsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.WarningId)
            .IsRequired(false);

        builder.HasOne(f => f.Warning)
            .WithMany(w => w.FileAttachments)
            .HasForeignKey(f => f.WarningId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.CompensationOrderId)
            .IsRequired(false);

        builder.HasOne(f => f.CompensationOrder)
            .WithMany(c => c.FileAttachments)
            .HasForeignKey(f => f.CompensationOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.VacationCompensationApplicationId)
            .IsRequired(false);

        builder.HasOne(f => f.VacationCompensationApplication)
            .WithMany(v => v.FileAttachments)
            .HasForeignKey(f => f.VacationCompensationApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.UnpaidLeaveApplicationId)
            .IsRequired(false);

        builder.HasOne(f => f.UnpaidLeaveApplication)
            .WithMany(u => u.FileAttachments)
            .HasForeignKey(f => f.UnpaidLeaveApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.UnpaidLeaveOrderId)
            .IsRequired(false);

        builder.HasOne(f => f.UnpaidLeaveOrder)
            .WithMany(u => u.FileAttachments)
            .HasForeignKey(f => f.UnpaidLeaveOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.EducationLeaveApplicationId)
            .IsRequired(false);

        builder.HasOne(f => f.EducationLeaveApplication)
            .WithMany(e => e.FileAttachments)
            .HasForeignKey(f => f.EducationLeaveApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.EducationLeaveOrderId)
            .IsRequired(false);

        builder.HasOne(f => f.EducationLeaveOrder)
            .WithMany(e => e.FileAttachments)
            .HasForeignKey(f => f.EducationLeaveOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.VacationApplicationId)
            .IsRequired(false);

        builder.HasOne(f => f.VacationApplication)
            .WithMany(v => v.FileAttachments)
            .HasForeignKey(f => f.VacationApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.VacationOrderId)
            .IsRequired(false);

        builder.HasOne(f => f.VacationOrder)
            .WithMany(v => v.FileAttachments)
            .HasForeignKey(f => f.VacationOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.BonusOrderId)
            .IsRequired(false);

        builder.HasOne(f => f.BonusOrder)
            .WithMany(b => b.FileAttachments)
            .HasForeignKey(f => f.BonusOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.SalaryDeductionId)
            .IsRequired(false);

        builder.HasOne(f => f.SalaryDeduction)
            .WithMany(s => s.FileAttachments)
            .HasForeignKey(f => f.SalaryDeductionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.EmploymentStatusChangeApplicationId)
            .IsRequired(false);

        builder.HasOne(f => f.EmploymentStatusChangeApplication)
            .WithMany(e => e.FileAttachments)
            .HasForeignKey(f => f.EmploymentStatusChangeApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.EmploymentStatusChangeOrderId)
            .IsRequired(false);

        builder.HasOne(f => f.EmploymentStatusChangeOrder)
            .WithMany(e => e.FileAttachments)
            .HasForeignKey(f => f.EmploymentStatusChangeOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.WorkOnNonWorkdayOrderId)
            .IsRequired(false);

        builder.HasOne(f => f.WorkOnNonWorkdayOrder)
            .WithMany(w => w.FileAttachments)
            .HasForeignKey(f => f.WorkOnNonWorkdayOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.VacationReturnApplicationId)
            .IsRequired(false);

        builder.HasOne(f => f.VacationReturnApplication)
            .WithMany(v => v.FileAttachments)
            .HasForeignKey(f => f.VacationReturnApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.VacationReturnOrderId)
            .IsRequired(false);

        builder.HasOne(f => f.VacationReturnOrder)
            .WithMany(v => v.FileAttachments)
            .HasForeignKey(f => f.VacationReturnOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.SearchVector)
            .HasColumnType("tsvector");

        builder.Property(e => e.SearchVector)
            .HasComputedColumnSql(
                "to_tsvector('simple', \"FileName\")",
                stored: true);

        builder.HasIndex(f => f.SearchVector)
            .HasMethod("GIN");

        builder.HasIndex(f => f.CreatedAt);
    }
}
