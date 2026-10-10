using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.Warnings.AddWarning;

internal sealed class AddWarningCommandHandler : ICommandHandler<AddWarningCommand, AddWarningResponse>
{
    private readonly IWarningRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public AddWarningCommandHandler(
        IWarningRepository repository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<AddWarningResponse>> Handle(AddWarningCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);

            var warning = WarningOrder.Create(
                request.DisciplinaryType,
                request.EmployeeId,
                request.OrderExecutionSupervisorId,
                request.SetDate,
                _userContext.UserId);

            await _repository.AddAsync(warning, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new AddWarningResponse
            {
                Id = warning.Id,
                OrderNumber = warning.OrderNumber
            });
        }
        catch (Exception ex)
        {
            return Result.Failure<AddWarningResponse>(
                WarningErrors.AddFailed);
        }
    }
}
