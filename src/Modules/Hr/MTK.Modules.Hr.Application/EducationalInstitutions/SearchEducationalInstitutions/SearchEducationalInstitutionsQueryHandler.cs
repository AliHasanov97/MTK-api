using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EducationalInstitutions;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.SearchEducationalInstitutions;

internal sealed class SearchEducationalInstitutionsQueryHandler : IQueryHandler<SearchEducationalInstitutionsQuery, SearchEducationalInstitutionsResponse>
{
    private readonly IEducationalInstitutionRepository _repository;
    private readonly IMapper _mapper;

    public SearchEducationalInstitutionsQueryHandler(IEducationalInstitutionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchEducationalInstitutionsResponse>> Handle(
        SearchEducationalInstitutionsQuery request,
        CancellationToken cancellationToken)
    {
        var institutions = await _repository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _repository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var data = _mapper.Map<List<SearchEducationalInstitutionsResponseItem>>(institutions);
        return Result.Success(new SearchEducationalInstitutionsResponse(data, totalCount, request.Page, request.PageSize));
    }
}
