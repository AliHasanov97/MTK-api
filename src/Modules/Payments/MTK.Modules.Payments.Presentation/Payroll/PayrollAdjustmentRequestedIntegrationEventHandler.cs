using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.IntegrationEvents.Payroll;
using MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollAdjustment;
using PaymentsAdjustmentReason = MTK.Modules.Payments.Domain.Payroll.PayrollAdjustmentReason;

namespace MTK.Modules.Payments.Presentation.Payroll;

internal sealed class PayrollAdjustmentRequestedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<PayrollAdjustmentRequestedIntegrationEvent>
{
    public override async Task Handle(
        PayrollAdjustmentRequestedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncPayrollAdjustmentCommand(
            integrationEvent.OrderId,
            integrationEvent.EmployeeId,
            // HR və Payments enum dəyərləri eynidir (bax: hər iki enum-un təyini).
            (PaymentsAdjustmentReason)integrationEvent.Reason,
            integrationEvent.Hours,
            integrationEvent.NormHours,
            integrationEvent.SalaryMonth,
            integrationEvent.SalaryYear);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync payroll adjustment: {result.Error}");
        }
    }
}
