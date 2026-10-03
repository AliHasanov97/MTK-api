using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.OwnershipHistories.Queries.GetOwnershipHistoryByApartment;

internal sealed class GetOwnershipHistoryByApartmentQueryHandler
    : IQueryHandler<GetOwnershipHistoryByApartmentQuery, IReadOnlyCollection<OwnershipHistoryResponse>>
{
    private readonly IOwnershipHistoryRepository _ownershipHistoryRepository;
    private readonly IMapper _mapper;

    public GetOwnershipHistoryByApartmentQueryHandler(
        IOwnershipHistoryRepository ownershipHistoryRepository,
        IMapper mapper)
    {
        _ownershipHistoryRepository = ownershipHistoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<OwnershipHistoryResponse>>> Handle(
        GetOwnershipHistoryByApartmentQuery request,
        CancellationToken cancellationToken)
    {
        var histories = await _ownershipHistoryRepository.GetByApartmentIdAsync(request.ApartmentId, cancellationToken);

        var ordered = histories.OrderBy(h => h.TransferDate).ToList();

        return _mapper.Map<List<OwnershipHistoryResponse>>(ordered);
    }
}
