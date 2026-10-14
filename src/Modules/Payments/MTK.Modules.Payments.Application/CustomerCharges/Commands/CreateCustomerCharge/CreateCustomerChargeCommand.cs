using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.CustomerCharges.Commands.CreateCustomerCharge;

/// <summary>
/// Müştəriyə müqavilədən kənar birdəfəlik borc yazır (məs. əlavə xidmət, cərimə).
/// Müştərinin əvvəlki avansı varsa, borc avtomatik ondan bağlanır.
/// </summary>
public sealed record CreateCustomerChargeCommand(
    Guid CustomerId,
    string Description,
    decimal Amount,
    DateTimeOffset? DueDate = null) : ICommand<Guid>;
