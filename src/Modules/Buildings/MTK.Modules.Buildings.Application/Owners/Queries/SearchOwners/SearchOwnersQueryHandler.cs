using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Owners.Queries.SearchOwners;

internal sealed class SearchOwnersQueryHandler
    : IQueryHandler<SearchOwnersQuery, SearchOwnersResponse>
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IMapper _mapper;

    public SearchOwnersQueryHandler(IOwnerRepository ownerRepository, IMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _mapper = mapper;
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

        var items = _mapper.Map<List<OwnerListItem>>(owners);

        var response = new SearchOwnersResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return Result.Success(response);
    }
}
