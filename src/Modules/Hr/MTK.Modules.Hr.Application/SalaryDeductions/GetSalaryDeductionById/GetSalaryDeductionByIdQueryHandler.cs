using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.SalaryDeductions;

namespace MTK.Modules.Hr.Application.SalaryDeductions.GetSalaryDeductionById;

internal sealed class GetSalaryDeductionByIdQueryHandler
    : IQueryHandler<GetSalaryDeductionByIdQuery, GetSalaryDeductionByIdResponse>
{
    private readonly ISalaryDeductionRepository _repository;
    private readonly IMapper _mapper;

    public GetSalaryDeductionByIdQueryHandler(
        ISalaryDeductionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetSalaryDeductionByIdResponse>> Handle(
        GetSalaryDeductionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var salaryDeduction = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (salaryDeduction is null)
        {
            return Result.Failure<GetSalaryDeductionByIdResponse>(SalaryDeductionErrors.NotFound);
        }

        return Result.Success(_mapper.Map<GetSalaryDeductionByIdResponse>(salaryDeduction));
    }
}
