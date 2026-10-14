using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.IntegrationEvents.BonusOrders;
using MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollBonus;

namespace MTK.Modules.Payments.Presentation.Payroll;

internal sealed class BonusOrderCreatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<BonusOrderCreatedIntegrationEvent>
{
    public override async Task Handle(
        BonusOrderCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncPayrollBonusCommand(
            integrationEvent.BonusOrderId,
            integrationEvent.EmployeeId,
            integrationEvent.BonusAmount,
            integrationEvent.SalaryMonth,
            integrationEvent.SalaryYear);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync bonus order: {result.Error}");
        }
    }
}
