namespace MTK.Modules.Hr.Application.Abstractions.Services.ExportService;

public interface IExportService
{
    Task<ExportResult> ExportJobApplicationToPdfAsync(
        Guid jobApplicationId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportEmploymentOrderToPdfAsync(
        Guid employmentOrderId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportApplicationForChangeOfPositionToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportOrderForChangeOfPositionToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportUnexcusedAbsenceToPdfAsync(
        Guid unexcusedAbsenceId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportWarningToPdfAsync(
        Guid warningId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportNoticeOfChangeInWorkingConditionsToWordAsync(
        Guid noticeId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportVacationCompensationApplicationToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportCompensationOrderToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportUnpaidLeaveApplicationToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportUnpaidLeaveOrderToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportVacationOrderToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportVacationApplicationToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportEducationLeaveApplicationToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportEducationLeaveOrderToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportBonusOrderToPdfAsync(
        Guid bonusOrderId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportSalaryDeductionToPdfAsync(
        Guid salaryDeductionId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportWorkOnNonWorkdayOrderToPdfAsync(
        Guid workOnNonWorkdayOrderId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportEmploymentStatusChangeOrderToPdfAsync(
        Guid employmentStatusChangeOrderId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportEmploymentStatusChangeApplicationToPdfAsync(
        Guid employmentStatusChangeApplicationId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportVacationReturnApplicationToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    Task<ExportResult> ExportVacationReturnOrderToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);
}
