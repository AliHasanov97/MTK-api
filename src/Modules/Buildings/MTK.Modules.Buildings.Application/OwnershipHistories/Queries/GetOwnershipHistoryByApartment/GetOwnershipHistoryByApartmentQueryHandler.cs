using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.OwnershipHistories.Queries.GetOwnershipHistoryByApartment;

internal sealed class GetOwnershipHistoryByApartmentQueryHandler
    : IQueryHandler<GetOwnershipHistoryByApartmentQuery, IReadOnlyCollection<OwnershipHistoryResponse>>
{
    private readonly IOwnershipHistoryRepository _ownershipHistoryRepository;

    public GetOwnershipHistoryByApartmentQueryHandler(IOwnershipHistoryRepository ownershipHistoryRepository)
    {
        _ownershipHistoryRepository = ownershipHistoryRepository;
    }

    public async Task<Result<IReadOnlyCollection<OwnershipHistoryResponse>>> Handle(
        GetOwnershipHistoryByApartmentQuery request,
        CancellationToken cancellationToken)
    {
        var histories = await _ownershipHistoryRepository.GetByApartmentIdAsync(request.ApartmentId, cancellationToken);

        var response = histories
            .Select(h => new OwnershipHistoryResponse(
                h.Id,
                h.PreviousOwnerId,
                h.PreviousOwnerName,
                h.NewOwnerId,
                h.NewOwnerName,
                h.TransferDate))
            .OrderBy(h => h.TransferDate)
            .ToList();

        return response;
    }
}
