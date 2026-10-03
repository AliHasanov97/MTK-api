using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetChargesByOwner;

internal sealed class GetChargesByOwnerQueryHandler : IQueryHandler<GetChargesByOwnerQuery, IReadOnlyCollection<ChargeResponse>>
{
    private readonly IChargeRepository _chargeRepository;
    private readonly IMapper _mapper;

    public GetChargesByOwnerQueryHandler(IChargeRepository chargeRepository, IMapper mapper)
    {
        _chargeRepository = chargeRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<ChargeResponse>>> Handle(
        GetChargesByOwnerQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _chargeRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);

        return _mapper.Map<List<ChargeResponse>>(charges);
    }
}
