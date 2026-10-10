using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.UnexcusedAbsences;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.GetUnexcusedAbsenceById;

internal sealed class GetUnexcusedAbsenceByIdQueryHandler : IQueryHandler<GetUnexcusedAbsenceByIdQuery, GetUnexcusedAbsenceByIdResponse>
{
    private readonly IUnexcusedAbsenceRepository _repository;
    private readonly IMapper _mapper;

    public GetUnexcusedAbsenceByIdQueryHandler(
        IUnexcusedAbsenceRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetUnexcusedAbsenceByIdResponse>> Handle(GetUnexcusedAbsenceByIdQuery request, CancellationToken cancellationToken)
    {
        var absence = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (absence is null)
            return Result.Failure<GetUnexcusedAbsenceByIdResponse>(UnexcusedAbsenceErrors.NotFound);

        return Result.Success(_mapper.Map<GetUnexcusedAbsenceByIdResponse>(absence));
    }
}
