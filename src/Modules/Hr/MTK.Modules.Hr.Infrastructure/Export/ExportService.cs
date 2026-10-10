using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class ExportService : IExportService
{
    private readonly JobApplicationExportService _jobApplicationExportService;
    private readonly EmploymentOrderExportService _employmentOrderExportService;
    private readonly ApplicationForChangeOfPositionExportService _applicationForChangeOfPositionExportService;
    private readonly OrderForChangeOfPositionExportService _orderForChangeOfPositionExportService;
    private readonly UnexcusedAbsenceExportService _unexcusedAbsenceExportService;
    private readonly WarningExportService _warningExportService;
    private readonly NoticeOfChangeInWorkingConditionsWordExportService _noticeOfChangeInWorkingConditionsWordExportService;
    private readonly VacationCompensationApplicationExportService _vacationCompensationApplicationExportService;
    private readonly CompensationOrderExportService _compensationOrderExportService;
    private readonly UnpaidLeaveApplicationExportService _unpaidLeaveApplicationExportService;
    private readonly UnpaidLeaveOrderExportService _unpaidLeaveOrderExportService;
    private readonly VacationApplicationExportService _vacationApplicationExportService;
    private readonly EducationLeaveApplicationExportService _educationLeaveApplicationExportService;
    private readonly EducationLeaveOrderExportService _educationLeaveOrderExportService;
    private readonly BonusOrderExportService _bonusOrderExportService;
    private readonly SalaryDeductionExportService _salaryDeductionExportService;
    private readonly WorkOnNonWorkdayOrderExportService _workOnNonWorkdayOrderExportService;
    private readonly EmploymentStatusChangeOrderExportService _employmentStatusChangeOrderExportService;
    private readonly EmploymentStatusChangeApplicationExportService _employmentStatusChangeApplicationExportService;
    private readonly VacationReturnApplicationExportService _vacationReturnApplicationExportService;
    private readonly VacationReturnOrderExportService _vacationReturnOrderExportService;

    public ExportService(
        JobApplicationExportService jobApplicationExportService,
        EmploymentOrderExportService employmentOrderExportService,
        ApplicationForChangeOfPositionExportService applicationForChangeOfPositionExportService,
        OrderForChangeOfPositionExportService orderForChangeOfPositionExportService,
        UnexcusedAbsenceExportService unexcusedAbsenceExportService,
        WarningExportService warningExportService,
        NoticeOfChangeInWorkingConditionsWordExportService noticeOfChangeInWorkingConditionsWordExportService,
        VacationCompensationApplicationExportService vacationCompensationApplicationExportService,
        CompensationOrderExportService compensationOrderExportService,
        UnpaidLeaveApplicationExportService unpaidLeaveApplicationExportService,
        UnpaidLeaveOrderExportService unpaidLeaveOrderExportService,
        VacationApplicationExportService vacationApplicationExportService,
        EducationLeaveApplicationExportService educationLeaveApplicationExportService,
        EducationLeaveOrderExportService educationLeaveOrderExportService,
        BonusOrderExportService bonusOrderExportService,
        SalaryDeductionExportService salaryDeductionExportService,
        WorkOnNonWorkdayOrderExportService workOnNonWorkdayOrderExportService,
        EmploymentStatusChangeOrderExportService employmentStatusChangeOrderExportService,
        EmploymentStatusChangeApplicationExportService employmentStatusChangeApplicationExportService,
        VacationReturnApplicationExportService vacationReturnApplicationExportService,
        VacationReturnOrderExportService vacationReturnOrderExportService)
    {
        _jobApplicationExportService = jobApplicationExportService;
        _employmentOrderExportService = employmentOrderExportService;
        _applicationForChangeOfPositionExportService = applicationForChangeOfPositionExportService;
        _orderForChangeOfPositionExportService = orderForChangeOfPositionExportService;
        _unexcusedAbsenceExportService = unexcusedAbsenceExportService;
        _warningExportService = warningExportService;
        _noticeOfChangeInWorkingConditionsWordExportService = noticeOfChangeInWorkingConditionsWordExportService;
        _vacationCompensationApplicationExportService = vacationCompensationApplicationExportService;
        _compensationOrderExportService = compensationOrderExportService;
        _unpaidLeaveApplicationExportService = unpaidLeaveApplicationExportService;
        _unpaidLeaveOrderExportService = unpaidLeaveOrderExportService;
        _vacationApplicationExportService = vacationApplicationExportService;
        _educationLeaveApplicationExportService = educationLeaveApplicationExportService;
        _educationLeaveOrderExportService = educationLeaveOrderExportService;
        _bonusOrderExportService = bonusOrderExportService;
        _salaryDeductionExportService = salaryDeductionExportService;
        _workOnNonWorkdayOrderExportService = workOnNonWorkdayOrderExportService;
        _employmentStatusChangeOrderExportService = employmentStatusChangeOrderExportService;
        _employmentStatusChangeApplicationExportService = employmentStatusChangeApplicationExportService;
        _vacationReturnApplicationExportService = vacationReturnApplicationExportService;
        _vacationReturnOrderExportService = vacationReturnOrderExportService;
    }

    public async Task<ExportResult> ExportJobApplicationToPdfAsync(
        Guid jobApplicationId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _jobApplicationExportService.ExportToPdfAsync(
            jobApplicationId,
            cancellationToken);

        var fileName = $"Ərizə_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportEmploymentOrderToPdfAsync(
        Guid employmentOrderId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _employmentOrderExportService.ExportToPdfAsync(
            employmentOrderId,
            cancellationToken);

        var fileName = $"Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportApplicationForChangeOfPositionToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _applicationForChangeOfPositionExportService.ExportToPdfAsync(
            applicationId,
            cancellationToken);

        var fileName = $"Keçid_Ərizə_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportOrderForChangeOfPositionToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _orderForChangeOfPositionExportService.ExportToPdfAsync(
            orderId,
            cancellationToken);

        var fileName = $"Keçid_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportUnexcusedAbsenceToPdfAsync(
        Guid unexcusedAbsenceId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _unexcusedAbsenceExportService.ExportToPdfAsync(
            unexcusedAbsenceId,
            cancellationToken);

        var fileName = $"Üzrsüz_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportWarningToPdfAsync(
        Guid warningId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index, disciplinaryType) = await _warningExportService.ExportToPdfAsync(
            warningId,
            cancellationToken);

        var fileNamePrefix = GetDisciplinaryFileNamePrefix(disciplinaryType);
        var fileName = $"{fileNamePrefix}_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    private static string GetDisciplinaryFileNamePrefix(DisciplinaryType disciplinaryType)
    {
        return disciplinaryType switch
        {
            DisciplinaryType.Warning => "Xəbərdarlıq",
            DisciplinaryType.Reprimand => "Töhmət",
            DisciplinaryType.SevereReprimand => "Şiddətli_Töhmət",
            _ => "Xəbərdarlıq"
        };
    }

    public async Task<ExportResult> ExportNoticeOfChangeInWorkingConditionsToWordAsync(
        Guid noticeId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _noticeOfChangeInWorkingConditionsWordExportService.ExportToWordAsync(
            noticeId,
            cancellationToken);

        var fileName = $"Xəbərdarlıq_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.docx";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportVacationCompensationApplicationToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _vacationCompensationApplicationExportService.ExportToPdfAsync(
            applicationId,
            cancellationToken);

        var fileName = $"Kompensasiya_Ərizə_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportCompensationOrderToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _compensationOrderExportService.ExportToPdfAsync(
            orderId,
            cancellationToken);

        var fileName = $"Kompensasiya_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportUnpaidLeaveApplicationToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _unpaidLeaveApplicationExportService.ExportToPdfAsync(
            applicationId,
            cancellationToken);

        var fileName = $"Ödənişsiz_Məzuniyyət_Ərizə_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportUnpaidLeaveOrderToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _unpaidLeaveOrderExportService.ExportToPdfAsync(
            orderId,
            cancellationToken);

        var fileName = $"Ödənişsiz_Məzuniyyət_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportVacationApplicationToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _vacationApplicationExportService.ExportToPdfAsync(
            applicationId,
            cancellationToken);

        var fileName = $"Ödənişli_Məzuniyyət_Ərizə_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportEducationLeaveApplicationToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _educationLeaveApplicationExportService.ExportToPdfAsync(
            applicationId,
            cancellationToken);

        var fileName = $"Təhsil_Məzuniyyəti_Ərizə_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportEducationLeaveOrderToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _educationLeaveOrderExportService.ExportToPdfAsync(
            orderId,
            cancellationToken);

        var fileName = $"Təhsil_Məzuniyyəti_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportBonusOrderToPdfAsync(
        Guid bonusOrderId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _bonusOrderExportService.ExportToPdfAsync(
            bonusOrderId,
            cancellationToken);

        var fileName = $"Mükafat_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportSalaryDeductionToPdfAsync(
        Guid salaryDeductionId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _salaryDeductionExportService.ExportToPdfAsync(
            salaryDeductionId,
            cancellationToken);

        var fileName = $"Əmək_Haqqından_Tutulma_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportWorkOnNonWorkdayOrderToPdfAsync(
        Guid workOnNonWorkdayOrderId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _workOnNonWorkdayOrderExportService.ExportToPdfAsync(
            workOnNonWorkdayOrderId,
            cancellationToken);

        var fileName = $"Qeyri_İş_Günü_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportEmploymentStatusChangeOrderToPdfAsync(
        Guid employmentStatusChangeOrderId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _employmentStatusChangeOrderExportService.ExportToPdfAsync(
            employmentStatusChangeOrderId,
            cancellationToken);

        var fileName = $"İş_Rejimi_Dəyişikliyi_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportEmploymentStatusChangeApplicationToPdfAsync(
        Guid employmentStatusChangeApplicationId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _employmentStatusChangeApplicationExportService.ExportToPdfAsync(
            employmentStatusChangeApplicationId,
            cancellationToken);

        var fileName = $"İş_Rejimi_Dəyişikliyi_Ərizə_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportVacationReturnApplicationToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _vacationReturnApplicationExportService.ExportToPdfAsync(
            applicationId,
            cancellationToken);

        var fileName = $"Məzuniyyətdən_Qayıtma_Ərizə_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }

    public async Task<ExportResult> ExportVacationReturnOrderToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var (stream, index) = await _vacationReturnOrderExportService.ExportToPdfAsync(
            orderId,
            cancellationToken);

        var fileName = $"Məzuniyyətdən_Qayıtma_Əmr_{index:D4}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return new ExportResult(stream, fileName);
    }
}