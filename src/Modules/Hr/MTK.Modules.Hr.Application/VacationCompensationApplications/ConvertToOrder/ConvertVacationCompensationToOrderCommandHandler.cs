using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.CompensationOrders;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.ConvertToOrder;

internal sealed class ConvertVacationCompensationToOrderCommandHandler
    : ICommandHandler<ConvertVacationCompensationToOrderCommand, ConvertVacationCompensationToOrderResponse>
{
    private readonly IVacationCompensationApplicationRepository _compensationApplicationRepository;
    private readonly ICompensationOrderRepository _compensationOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public ConvertVacationCompensationToOrderCommandHandler(
        IVacationCompensationApplicationRepository compensationApplicationRepository,
        ICompensationOrderRepository compensationOrderRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        IMapper mapper)
    {
        _compensationApplicationRepository = compensationApplicationRepository;
        _compensationOrderRepository = compensationOrderRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<ConvertVacationCompensationToOrderResponse>> Handle(
        ConvertVacationCompensationToOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. VacationCompensationApplication götür
        var application = await _compensationApplicationRepository.GetByIdWithLinesAsync(
            request.CompensationApplicationId,
            cancellationToken);

        if (application == null)
            return Result.Failure<ConvertVacationCompensationToOrderResponse>(
                VacationCompensationApplicationErrors.NotFound);

        // 2. Artıq order yaradılıbmı?
        var existingOrder = await _compensationOrderRepository.GetByCompensationApplicationIdAsync(
            request.CompensationApplicationId,
            cancellationToken);

        if (existingOrder != null)
            return Result.Failure<ConvertVacationCompensationToOrderResponse>(
                VacationCompensationApplicationErrors.AlreadyConvertedToOrder);

        // 3. CompensationOrder yarat
        var order = CompensationOrder.Create(
            application.Id,
            application.EmployeeId,
            application.RequestedDays,
            _userContext.UserId,
            application.Notes,
            application.WorkYearStart,
            application.WorkYearEnd
        );

        // 4. Saxla
        await _compensationOrderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Response - Employee və Company ilə birlikdə yüklə
        var savedOrder = await _compensationOrderRepository.GetByIdWithEmployeeAndCompanyAsync(
            order.Id,
            cancellationToken);

        // 6. AutoMapper ilə response yarat
        return Result.Success(_mapper.Map<ConvertVacationCompensationToOrderResponse>(savedOrder));
    }
}