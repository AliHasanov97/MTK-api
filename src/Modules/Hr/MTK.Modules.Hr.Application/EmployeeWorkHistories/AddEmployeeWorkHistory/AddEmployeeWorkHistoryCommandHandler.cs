using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.AddEmployeeWorkHistory;

internal sealed class AddEmployeeWorkHistoryCommandHandler
    : ICommandHandler<AddEmployeeWorkHistoryCommand, AddEmployeeWorkHistoryResponse>
{
    private readonly IEmployeeWorkHistoryRepository _workHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public AddEmployeeWorkHistoryCommandHandler(
        IEmployeeWorkHistoryRepository workHistoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _workHistoryRepository = workHistoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<AddEmployeeWorkHistoryResponse>> Handle(
        AddEmployeeWorkHistoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.EndDate.HasValue && request.EndDate.Value < request.StartDate)
                return Result.Failure<AddEmployeeWorkHistoryResponse>(EmployeeWorkHistoryErrors.InvalidDateRange);

            var workHistory = EmployeeWorkHistory.Create(
                employeeId: request.EmployeeId,
                companyName: request.CompanyName,
                position: request.Position,
                startDate: request.StartDate,
                endDate: request.EndDate,
                notes: request.Notes,
                createdById: _userContext.UserId);

            await _workHistoryRepository.AddAsync(workHistory, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<AddEmployeeWorkHistoryResponse>(workHistory);
            return Result.Success(response);
        }
        catch (Exception)
        {
            return Result.Failure<AddEmployeeWorkHistoryResponse>(
                EmployeeWorkHistoryErrors.AddFailed);
        }
    }
}
