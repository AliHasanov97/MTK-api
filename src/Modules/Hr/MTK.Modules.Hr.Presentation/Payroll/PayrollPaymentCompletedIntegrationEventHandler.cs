using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.SalaryDeductions.Commands.RecordPayment;
using MTK.Modules.Payments.IntegrationEvents.Payroll;

namespace MTK.Modules.Hr.Presentation.Payroll;

internal sealed class PayrollPaymentCompletedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<PayrollPaymentCompletedIntegrationEvent>
{
    public override async Task Handle(
        PayrollPaymentCompletedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new RecordSalaryDeductionPaymentCommand(
            integrationEvent.EmployeeId,
            integrationEvent.CourtDeductionAmount,
            integrationEvent.SalaryMonth,
            integrationEvent.SalaryYear);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to record salary deduction payment: {result.Error}");
        }
    }
}
