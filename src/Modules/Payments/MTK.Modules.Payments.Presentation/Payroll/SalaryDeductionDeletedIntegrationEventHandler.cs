using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.IntegrationEvents.SalaryDeductions;
using MTK.Modules.Payments.Application.Payroll.Commands.RemovePayrollDeduction;

namespace MTK.Modules.Payments.Presentation.Payroll;

internal sealed class SalaryDeductionDeletedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<SalaryDeductionDeletedIntegrationEvent>
{
    public override async Task Handle(
        SalaryDeductionDeletedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new RemovePayrollDeductionCommand(integrationEvent.SalaryDeductionId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to remove salary deduction: {result.Error}");
        }
    }
}
