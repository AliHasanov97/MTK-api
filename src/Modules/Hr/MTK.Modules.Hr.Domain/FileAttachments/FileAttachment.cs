using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Domain.BonusOrders;
using MTK.Modules.Hr.Domain.CompensationOrders;
using MTK.Modules.Hr.Domain.SalaryDeductions;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmploymentOrders;
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
using MTK.Modules.Hr.Domain.Warnings;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using MTK.Modules.Hr.Domain.VacationReturnOrders;

namespace MTK.Modules.Hr.Domain.FileAttachments;

public class FileAttachment : FileAttachmentBase
{
    private FileAttachment() { }

    public FileAttachment(Guid id, string fileName, string mimeType, bool isPublic)
    {
        Id = id;
        FileName = fileName;
        MimeType = mimeType;
        IsPublic = isPublic;
    }

    public Guid? JobApplicationId { get; set; }
    public JobApplications.JobApplication? JobApplication { get; set; }

    public Guid? EmploymentOrderId { get; set; }
    public EmploymentOrder? EmploymentOrder { get; set; }

    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public Guid? ApplicationForChangeOfPositionId { get; set; }
    public ApplicationForChangeOfPosition? ApplicationForChangeOfPosition { get; set; }

    public Guid? OrderForChangeOfPositionId { get; set; }
    public OrderForChangeOfPosition? OrderForChangeOfPosition { get; set; }

    public Guid? UnexcusedAbsenceId { get; set; }
    public UnexcusedAbsenceOrder? UnexcusedAbsence { get; set; }

    public Guid? NoticeOfChangeInWorkingConditionsId { get; set; }
    public NoticeOfChangeInWorkingConditions? NoticeOfChangeInWorkingConditions { get; set; }

    public Guid? WarningId { get; set; }
    public WarningOrder? Warning { get; set; }

    public Guid? CompensationOrderId { get; set; }
    public CompensationOrder? CompensationOrder { get; set; }

    public Guid? VacationCompensationApplicationId { get; set; }
    public VacationCompensationApplication? VacationCompensationApplication { get; set; }

    public Guid? UnpaidLeaveApplicationId { get; set; }
    public UnpaidLeaveApplication? UnpaidLeaveApplication { get; set; }

    public Guid? UnpaidLeaveOrderId { get; set; }
    public UnpaidLeaveOrder? UnpaidLeaveOrder { get; set; }

    public Guid? EducationLeaveApplicationId { get; set; }
    public EducationLeaveApplication? EducationLeaveApplication { get; set; }

    public Guid? EducationLeaveOrderId { get; set; }
    public EducationLeaveOrder? EducationLeaveOrder { get; set; }

    public Guid? VacationApplicationId { get; set; }
    public VacationApplication? VacationApplication { get; set; }

    public Guid? VacationOrderId { get; set; }
    public VacationOrder? VacationOrder { get; set; }

    public Guid? BonusOrderId { get; set; }
    public BonusOrder? BonusOrder { get; set; }

    public Guid? SalaryDeductionId { get; set; }
    public SalaryDeduction? SalaryDeduction { get; set; }

    public Guid? EmploymentStatusChangeApplicationId { get; set; }
    public EmploymentStatusChangeApplication? EmploymentStatusChangeApplication { get; set; }

    public Guid? EmploymentStatusChangeOrderId { get; set; }
    public EmploymentStatusChangeOrder? EmploymentStatusChangeOrder { get; set; }

    public Guid? WorkOnNonWorkdayOrderId { get; set; }
    public WorkOnNonWorkdayOrder? WorkOnNonWorkdayOrder { get; set; }

    public Guid? VacationReturnApplicationId { get; set; }
    public VacationReturnApplication? VacationReturnApplication { get; set; }

    public Guid? VacationReturnOrderId { get; set; }
    public VacationReturnOrder? VacationReturnOrder { get; set; }

    public DocumentType? DocumentType { get; private set; }

    public static FileAttachment Create(
        Guid id,
        string fileName,
        string mimeType,
        bool isPublic,
        Guid? jobApplicationId = null,
        Guid? employmentOrderId = null,
        Guid? employeeId = null,
        Guid? applicationForChangeOfPositionId = null,
        Guid? orderForChangeOfPositionId = null,
        Guid? unexcusedAbsenceId = null,
        Guid? noticeOfChangeInWorkingConditionsId = null,
        Guid? warningId = null,
        Guid? compensationOrderId = null,
        Guid? vacationCompensationApplicationId = null,
        Guid? unpaidLeaveApplicationId = null,
        Guid? unpaidLeaveOrderId = null,
        Guid? educationLeaveApplicationId = null,
        Guid? educationLeaveOrderId = null,
        Guid? vacationApplicationId = null,
        Guid? vacationOrderId = null,
        Guid? bonusOrderId = null,
        Guid? salaryDeductionId = null,
        Guid? employmentStatusChangeApplicationId = null,
        Guid? employmentStatusChangeOrderId = null,
        Guid? workOnNonWorkdayOrderId = null,
        Guid? vacationReturnApplicationId = null,
        Guid? vacationReturnOrderId = null,
        DocumentType? documentType = null)
    {
        if (!jobApplicationId.HasValue &&
            !employmentOrderId.HasValue &&
            !employeeId.HasValue &&
            !applicationForChangeOfPositionId.HasValue &&
            !orderForChangeOfPositionId.HasValue &&
            !unexcusedAbsenceId.HasValue &&
            !noticeOfChangeInWorkingConditionsId.HasValue &&
            !warningId.HasValue &&
            !compensationOrderId.HasValue &&
            !vacationCompensationApplicationId.HasValue &&
            !unpaidLeaveApplicationId.HasValue &&
            !unpaidLeaveOrderId.HasValue &&
            !educationLeaveApplicationId.HasValue &&
            !educationLeaveOrderId.HasValue &&
            !vacationApplicationId.HasValue &&
            !vacationOrderId.HasValue &&
            !bonusOrderId.HasValue &&
            !salaryDeductionId.HasValue &&
            !employmentStatusChangeApplicationId.HasValue &&
            !employmentStatusChangeOrderId.HasValue &&
            !workOnNonWorkdayOrderId.HasValue &&
            !vacationReturnApplicationId.HasValue &&
            !vacationReturnOrderId.HasValue)
        {
            throw new InvalidOperationException(
                "At least one entity ID must be provided.");
        }

        var attachment = new FileAttachment(id, fileName, mimeType, isPublic);
        attachment.JobApplicationId = jobApplicationId;
        attachment.EmploymentOrderId = employmentOrderId;
        attachment.EmployeeId = employeeId;
        attachment.ApplicationForChangeOfPositionId = applicationForChangeOfPositionId;
        attachment.OrderForChangeOfPositionId = orderForChangeOfPositionId;
        attachment.UnexcusedAbsenceId = unexcusedAbsenceId;
        attachment.NoticeOfChangeInWorkingConditionsId = noticeOfChangeInWorkingConditionsId;
        attachment.WarningId = warningId;
        attachment.CompensationOrderId = compensationOrderId;
        attachment.VacationCompensationApplicationId = vacationCompensationApplicationId;
        attachment.UnpaidLeaveApplicationId = unpaidLeaveApplicationId;
        attachment.UnpaidLeaveOrderId = unpaidLeaveOrderId;
        attachment.EducationLeaveApplicationId = educationLeaveApplicationId;
        attachment.EducationLeaveOrderId = educationLeaveOrderId;
        attachment.VacationApplicationId = vacationApplicationId;
        attachment.VacationOrderId = vacationOrderId;
        attachment.BonusOrderId = bonusOrderId;
        attachment.SalaryDeductionId = salaryDeductionId;
        attachment.EmploymentStatusChangeApplicationId = employmentStatusChangeApplicationId;
        attachment.EmploymentStatusChangeOrderId = employmentStatusChangeOrderId;
        attachment.WorkOnNonWorkdayOrderId = workOnNonWorkdayOrderId;
        attachment.VacationReturnApplicationId = vacationReturnApplicationId;
        attachment.VacationReturnOrderId = vacationReturnOrderId;
        attachment.DocumentType = documentType;
        return attachment;
    }
}
