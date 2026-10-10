using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EducationalInstitutions;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.GetEducationalInstitutionById;

internal sealed class GetEducationalInstitutionByIdQueryHandler
    : IQueryHandler<GetEducationalInstitutionByIdQuery, GetEducationalInstitutionByIdResponse>
{
    private readonly IEducationalInstitutionRepository _repository;
    private readonly IMapper _mapper;

    public GetEducationalInstitutionByIdQueryHandler(
        IEducationalInstitutionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetEducationalInstitutionByIdResponse>> Handle(
        GetEducationalInstitutionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var institution = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (institution is null)
            return Result.Failure<GetEducationalInstitutionByIdResponse>(EducationalInstitutionErrors.NotFound);

        return Result.Success(_mapper.Map<GetEducationalInstitutionByIdResponse>(institution));
    }
}
