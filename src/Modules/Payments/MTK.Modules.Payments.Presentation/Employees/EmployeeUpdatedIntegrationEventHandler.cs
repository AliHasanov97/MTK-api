using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.IntegrationEvents.Employees;
using MTK.Modules.Payments.Application.Employees.Commands.SyncEmployee;

namespace MTK.Modules.Payments.Presentation.Employees;

internal sealed class EmployeeUpdatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<EmployeeUpdatedIntegrationEvent>
{
    public override async Task Handle(
        EmployeeUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncEmployeeCommand(
            integrationEvent.EmployeeId,
            integrationEvent.FullName,
            integrationEvent.BaseSalary);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync employee: {result.Error}");
        }
    }
}
