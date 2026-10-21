using MTK.Common.Application.EventBus;

namespace MTK.Modules.Payments.IntegrationEvents.DebtReminders;

/// <summary>Two or more monthly resident charges remain unpaid.</summary>
public sealed class DebtReminderRequestedIntegrationEvent : IntegrationEvent
{
    public DebtReminderRequestedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid ownerId,
        string ownerName,
        string companyName,
        string email,
        IReadOnlyCollection<string> unpaidPeriods,
        decimal outstandingAmount,
        decimal totalDebt,
        decimal totalPaid,
        IReadOnlyCollection<DebtReminderPropertyDetail> properties)
        : base(integrationEventId, occurredOnUtc)
    {
        OwnerId = ownerId;
        OwnerName = ownerName;
        CompanyName = companyName;
        Email = email;
        UnpaidPeriods = unpaidPeriods.ToArray();
        OutstandingAmount = outstandingAmount;
        TotalDebt = totalDebt;
        TotalPaid = totalPaid;
        Properties = properties.ToArray();
    }

    public Guid OwnerId { get; }
    public string OwnerName { get; }
    public string CompanyName { get; }
    public string Email { get; }
    public IReadOnlyCollection<string> UnpaidPeriods { get; }
    public decimal OutstandingAmount { get; }
    public decimal TotalDebt { get; }
    public decimal TotalPaid { get; }
    public IReadOnlyCollection<DebtReminderPropertyDetail> Properties { get; }
}

public sealed record DebtReminderPropertyDetail(
    string PropertyLabel,
    IReadOnlyCollection<DebtReminderPeriodDetail> Periods,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal OutstandingAmount);

public sealed record DebtReminderPeriodDetail(
    string Period,
    decimal Amount,
    decimal PaidAmount,
    decimal OutstandingAmount);
