using MTK.Common.Domain.Queries;
using MTK.Modules.Hr.Domain.FileAttachments;

namespace MTK.Modules.Hr.Application.FileAttachments.SearchFileAttachments;

public class SearchFileAttachmentsResponse(List<SearchFileAttachmentsResponseItem> data, int totalCount, int page, int pageSize)
    : PagedListResponse<SearchFileAttachmentsResponseItem>(data, totalCount, page, pageSize);

public class SearchFileAttachmentsResponseItem
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string MimeType { get; set; }
    public bool IsPublic { get; set; }
    public DocumentType? DocumentType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
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
}
