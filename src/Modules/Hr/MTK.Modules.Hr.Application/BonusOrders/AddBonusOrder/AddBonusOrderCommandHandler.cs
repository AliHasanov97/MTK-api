using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.BonusOrders;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Application.BonusOrders.AddBonusOrder;

internal sealed class AddBonusOrderCommandHandler : ICommandHandler<AddBonusOrderCommand, AddBonusOrderResponse>
{
    private readonly IBonusOrderRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AddBonusOrderCommandHandler(
        IBonusOrderRepository repository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<AddBonusOrderResponse>> Handle(AddBonusOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);

            if (employee is null)
            {
                return Result.Failure<AddBonusOrderResponse>(EmployeeErrors.NotFound);
            }


            var bonusOrder = BonusOrder.Create(
                request.EmployeeId,
                request.OrderExecutionSupervisorId,
                request.BonusQuantity,
                request.SalaryMonth,
                request.SalaryYear,
                _userContext.UserId);

            await _repository.AddAsync(bonusOrder, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new AddBonusOrderResponse
            {
                Id = bonusOrder.Id,
                OrderNumber = bonusOrder.OrderNumber
            });
        }
        catch (Exception)
        {
            return Result.Failure<AddBonusOrderResponse>(BonusOrderErrors.AddFailed);
        }
    }
}
