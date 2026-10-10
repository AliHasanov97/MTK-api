using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.AddNoticeOfChangeInWorkingConditions;

internal sealed class AddNoticeOfChangeInWorkingConditionsCommandHandler : ICommandHandler<AddNoticeOfChangeInWorkingConditionsCommand, AddNoticeOfChangeInWorkingConditionsResponse>
{
    private readonly INoticeOfChangeInWorkingConditionsRepository _repository;
    private readonly IOrderRepository _orderRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public AddNoticeOfChangeInWorkingConditionsCommandHandler(
        INoticeOfChangeInWorkingConditionsRepository repository,
        IOrderRepository orderRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _repository = repository;
        _orderRepository = orderRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<AddNoticeOfChangeInWorkingConditionsResponse>> Handle(AddNoticeOfChangeInWorkingConditionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);

            var lastNoticeIndex = await _repository.CountAsync(null, null, null, cancellationToken);
            var noticeIndex = lastNoticeIndex + 1;
            
            var notice = NoticeOfChangeInWorkingConditions.Create(
                noticeIndex,
                request.EmployeeId,
                request.StartDate,
                _userContext.UserId);

            await _repository.AddAsync(notice, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<AddNoticeOfChangeInWorkingConditionsResponse>(notice));
        }
        catch (Exception)
        {
            return Result.Failure<AddNoticeOfChangeInWorkingConditionsResponse>(NoticeOfChangeInWorkingConditionsErrors.AddFailed);
        }
    }
}
