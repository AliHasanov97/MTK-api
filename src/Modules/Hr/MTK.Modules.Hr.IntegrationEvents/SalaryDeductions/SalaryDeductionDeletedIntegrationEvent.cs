using MTK.Common.Application.EventBus;

namespace MTK.Modules.Hr.IntegrationEvents.SalaryDeductions;

/// <summary>
/// Əmək haqqından tutma əmri silindikdə (borc ödənilib/qərar ləğv olunub) digər
/// modullara bildiriş — Payments bunu öz snapshot-undan çıxarır ki, gələcək
/// aylarda artıq tutulmasın.
/// </summary>
public sealed class SalaryDeductionDeletedIntegrationEvent : IntegrationEvent
{
    public SalaryDeductionDeletedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid salaryDeductionId,
        Guid employeeId)
        : base(integrationEventId, occurredOnUtc)
    {
        SalaryDeductionId = salaryDeductionId;
        EmployeeId = employeeId;
    }

    public Guid SalaryDeductionId { get; }
    public Guid EmployeeId { get; }
}
