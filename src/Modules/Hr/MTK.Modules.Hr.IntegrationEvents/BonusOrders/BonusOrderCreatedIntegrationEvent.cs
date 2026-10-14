using MTK.Common.Application.EventBus;

namespace MTK.Modules.Hr.IntegrationEvents.BonusOrders;

/// <summary>
/// Bir işçiyə konkret ay üçün mükafat (bonus) əmri verildikdə digər modullara
/// bildiriş — Payments bunu həmin ayın əmək haqqı hesablamasına (gross-a əlavə)
/// daxil etmək üçün consume edir.
/// </summary>
public sealed class BonusOrderCreatedIntegrationEvent : IntegrationEvent
{
    public BonusOrderCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid bonusOrderId,
        Guid employeeId,
        decimal bonusAmount,
        int salaryMonth,
        int salaryYear)
        : base(integrationEventId, occurredOnUtc)
    {
        BonusOrderId = bonusOrderId;
        EmployeeId = employeeId;
        BonusAmount = bonusAmount;
        SalaryMonth = salaryMonth;
        SalaryYear = salaryYear;
    }

    public Guid BonusOrderId { get; }
    public Guid EmployeeId { get; }
    public decimal BonusAmount { get; }
    public int SalaryMonth { get; }
    public int SalaryYear { get; }
}
