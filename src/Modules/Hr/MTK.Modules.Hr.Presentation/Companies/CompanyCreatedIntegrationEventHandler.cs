using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Companies.Commands.SyncCompany;
using MTK.Modules.Payments.IntegrationEvents.Companies;

namespace MTK.Modules.Hr.Presentation.Companies;

internal sealed class CompanyCreatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<CompanyCreatedIntegrationEvent>
{
    public override async Task Handle(
        CompanyCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncCompanyCommand(
            integrationEvent.CompanyId,
            integrationEvent.Name,
            integrationEvent.Director,
            integrationEvent.Address);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync company: {result.Error}");
        }
    }
}
