using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.UpdateEmployeeWorkHistory;

internal sealed class UpdateEmployeeWorkHistoryCommandHandler
    : ICommandHandler<UpdateEmployeeWorkHistoryCommand, UpdateEmployeeWorkHistoryResponse>
{
    private readonly IEmployeeWorkHistoryRepository _workHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateEmployeeWorkHistoryCommandHandler(
        IEmployeeWorkHistoryRepository workHistoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _workHistoryRepository = workHistoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UpdateEmployeeWorkHistoryResponse>> Handle(
        UpdateEmployeeWorkHistoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var workHistory = await _workHistoryRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
            if (workHistory is null)
                return Result.Failure<UpdateEmployeeWorkHistoryResponse>(EmployeeWorkHistoryErrors.NotFound);

            workHistory.Update(
                companyName: request.CompanyName,
                position: request.Position,
                startDate: request.StartDate,
                endDate: request.EndDate,
                notes: request.Notes);

            if (workHistory.EndDate.HasValue && workHistory.EndDate.Value < workHistory.StartDate)
                return Result.Failure<UpdateEmployeeWorkHistoryResponse>(EmployeeWorkHistoryErrors.InvalidDateRange);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<UpdateEmployeeWorkHistoryResponse>(workHistory);
            return Result.Success(response);
        }
        catch (NullReferenceException)
        {
            return Result.Failure<UpdateEmployeeWorkHistoryResponse>(EmployeeWorkHistoryErrors.NotFound);
        }
    }
}
