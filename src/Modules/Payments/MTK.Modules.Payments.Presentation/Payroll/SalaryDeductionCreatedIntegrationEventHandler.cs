using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.IntegrationEvents.SalaryDeductions;
using MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollDeduction;

namespace MTK.Modules.Payments.Presentation.Payroll;

internal sealed class SalaryDeductionCreatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<SalaryDeductionCreatedIntegrationEvent>
{
    public override async Task Handle(
        SalaryDeductionCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncPayrollDeductionCommand(
            integrationEvent.SalaryDeductionId,
            integrationEvent.EmployeeId,
            integrationEvent.PercentageSalary,
            integrationEvent.StartDate,
            integrationEvent.StateFee,
            integrationEvent.Debt);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync salary deduction: {result.Error}");
        }
    }
}
