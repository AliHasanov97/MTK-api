using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.IntegrationEvents.Employees;
using MTK.Modules.Payments.Application.Employees.Commands.SyncEmployee;

namespace MTK.Modules.Payments.Presentation.Employees;

internal sealed class EmployeeCreatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<EmployeeCreatedIntegrationEvent>
{
    public override async Task Handle(
        EmployeeCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncEmployeeCommand(
            integrationEvent.EmployeeId,
            integrationEvent.FullName,
            integrationEvent.BaseSalary,
            integrationEvent.IsActive);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync employee: {result.Error}");
        }
    }
}
