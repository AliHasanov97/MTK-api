using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Owners.Queries.SearchOwners;

internal sealed class SearchOwnersQueryHandler
    : IQueryHandler<SearchOwnersQuery, SearchOwnersResponse>
{
    private readonly IOwnerRepository _ownerRepository;

    public SearchOwnersQueryHandler(IOwnerRepository ownerRepository)
    {
        _ownerRepository = ownerRepository;
    }

    public async Task<Result<SearchOwnersResponse>> Handle(
        SearchOwnersQuery request,
        CancellationToken cancellationToken)
    {
        var owners = await _ownerRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _ownerRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = owners.Select(owner => new OwnerListItem(
            owner.Id,
            owner.UserId,
            owner.FullName,
            owner.Email,
            owner.PhoneNumber,
            owner.IsActive)).ToList();

        var response = new SearchOwnersResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return Result.Success(response);
    }
}
