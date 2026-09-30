using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetChargesByOwner;

internal sealed class GetChargesByOwnerQueryHandler : IQueryHandler<GetChargesByOwnerQuery, IReadOnlyCollection<ChargeResponse>>
{
    private readonly IChargeRepository _chargeRepository;

    public GetChargesByOwnerQueryHandler(IChargeRepository chargeRepository)
    {
        _chargeRepository = chargeRepository;
    }

    public async Task<Result<IReadOnlyCollection<ChargeResponse>>> Handle(
        GetChargesByOwnerQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _chargeRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);

        var response = charges
            .Select(c => new ChargeResponse(
                c.Id,
                c.OwnerId,
                c.PropertyType,
                c.PropertyId,
                c.Period,
                c.Amount,
                c.PaidAmount,
                c.Status,
                c.CreatedAt,
                c.Description,
                c.AreaSquareMeters,
                c.RateAmount,
                c.RateType,
                c.IssuedOn))
            .ToList();

        return response;
    }
}
