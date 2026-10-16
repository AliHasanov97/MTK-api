using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.IntegrationEvents.SalaryDeductions;
using MTK.Modules.Payments.Application.Payroll.Commands.UpdatePayrollDeduction;

namespace MTK.Modules.Payments.Presentation.Payroll;

internal sealed class SalaryDeductionUpdatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<SalaryDeductionUpdatedIntegrationEvent>
{
    public override async Task Handle(
        SalaryDeductionUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdatePayrollDeductionCommand(
            integrationEvent.SalaryDeductionId,
            integrationEvent.StateFeeRemaining,
            integrationEvent.DebtRemaining);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to update payroll deduction: {result.Error}");
        }
    }
}
