using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.FileAttachments;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MTK.Modules.Hr.Application.FileAttachments.AddBulkFileAttachment;

public class AddBulkFileAttachmentCommand : ICommand<AddBulkFileAttachmentResponse>
{
    [Required]
    public List<IFormFile> Files { get; set; } = new();

    public Guid? JobApplicationId { get; set; }
    public Guid? EmploymentOrderId { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid? ApplicationForChangeOfPositionId { get; set; }
    public Guid? OrderForChangeOfPositionId { get; set; }
    public Guid? UnexcusedAbsenceId { get; set; }
    public Guid? NoticeOfChangeInWorkingConditionsId { get; set; }
    public Guid? WarningId { get; set; }
    public Guid? CompensationOrderId { get; set; }
    public Guid? VacationCompensationApplicationId { get; set; }
    public Guid? UnpaidLeaveApplicationId { get; set; }
    public Guid? UnpaidLeaveOrderId { get; set; }
    public Guid? EducationLeaveApplicationId { get; set; }
    public Guid? EducationLeaveOrderId { get; set; }
    public Guid? VacationApplicationId { get; set; }
    public Guid? VacationOrderId { get; set; }
    public Guid? BonusOrderId { get; set; }
    public Guid? SalaryDeductionId { get; set; }
    public Guid? EmploymentStatusChangeApplicationId { get; set; }
    public Guid? EmploymentStatusChangeOrderId { get; set; }
    public Guid? WorkOnNonWorkdayOrderId { get; set; }
    public Guid? VacationReturnApplicationId { get; set; }
    public Guid? VacationReturnOrderId { get; set; }
    public DocumentType? DocumentType { get; set; }
}
