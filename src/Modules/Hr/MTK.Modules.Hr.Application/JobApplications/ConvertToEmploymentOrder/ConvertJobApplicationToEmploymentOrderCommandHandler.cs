using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.EmploymentOrders;
using MTK.Modules.Hr.Application.LaborCodeCases;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.EmploymentOrders;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Jobs;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Application.JobApplications.ConvertToEmploymentOrder;

internal sealed class ConvertJobApplicationToEmploymentOrderCommandHandler
    : ICommandHandler<ConvertJobApplicationToEmploymentOrderCommand, ConvertJobApplicationToEmploymentOrderResponse>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IEmploymentOrderRepository _employmentOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ConvertJobApplicationToEmploymentOrderCommandHandler(
        IJobApplicationRepository jobApplicationRepository,
        IEmploymentOrderRepository employmentOrderRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _employmentOrderRepository = employmentOrderRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<ConvertJobApplicationToEmploymentOrderResponse>> Handle(
        ConvertJobApplicationToEmploymentOrderCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var jobApplication = await _jobApplicationRepository.GetByIdDefaultAsync(request.JobApplicationId, cancellationToken);
            if (jobApplication is null)
                return Result.Failure<ConvertJobApplicationToEmploymentOrderResponse>(JobApplicationErrors.NotFound);

            if (jobApplication.Status == ApplicationStatus.ConvertedToOrder)
                return Result.Failure<ConvertJobApplicationToEmploymentOrderResponse>(JobApplicationErrors.AlreadyConverted);

            var employmentOrder = EmploymentOrder.Create(
                jobApplication.Id,
                request.StartDate,
                request.EndDate,
                _userContext.UserId,
                request.LaborCodeCaseId,
                jobApplication.Name,
                jobApplication.Surname,
                jobApplication.FathersName,
                jobApplication.Gender,
                jobApplication.JobId);

            jobApplication.MarkAsConvertedToOrder();

            _employmentOrderRepository.Add(employmentOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new ConvertJobApplicationToEmploymentOrderResponse
            {
                EmploymentOrderId = employmentOrder.Id,
                OrderNumber = employmentOrder.OrderNumber
            });
        }
        catch (NullReferenceException)
        {
            return Result.Failure<ConvertJobApplicationToEmploymentOrderResponse>(JobApplicationErrors.NotFound);
        }
    }
}
