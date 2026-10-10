using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.SearchUnexcusedAbsences;

internal sealed class SearchUnexcusedAbsencesQueryHandler : IQueryHandler<SearchUnexcusedAbsencesQuery, SearchUnexcusedAbsencesResponse>
{
    private readonly IUnexcusedAbsenceRepository _repository;
    private readonly IMapper _mapper;

    public SearchUnexcusedAbsencesQueryHandler(
        IUnexcusedAbsenceRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchUnexcusedAbsencesResponse>> Handle(SearchUnexcusedAbsencesQuery request, CancellationToken cancellationToken)
    {
        var absences = await _repository.SearchAsync(
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

        var data = _mapper.Map<List<SearchUnexcusedAbsencesResponseItem>>(absences);

        return Result.Success(new SearchUnexcusedAbsencesResponse(data, totalCount, request.Page ?? 0, request.PageSize ?? 100));
    }
}
