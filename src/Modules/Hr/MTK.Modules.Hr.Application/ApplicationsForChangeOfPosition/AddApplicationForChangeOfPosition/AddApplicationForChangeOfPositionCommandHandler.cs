using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.Jobs;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.AddApplicationForChangeOfPosition;

internal sealed class AddApplicationForChangeOfPositionCommandHandler
    : ICommandHandler<AddApplicationForChangeOfPositionCommand, AddApplicationForChangeOfPositionResponse>
{
    private readonly IApplicationForChangeOfPositionRepository _applicationRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public AddApplicationForChangeOfPositionCommandHandler(
        IApplicationForChangeOfPositionRepository applicationRepository,
        IEmployeeRepository employeeRepository,
        IJobRepository jobRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _applicationRepository = applicationRepository;
        _employeeRepository = employeeRepository;
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<AddApplicationForChangeOfPositionResponse>> Handle(
        AddApplicationForChangeOfPositionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get employee info (needed for the current position)
            var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);
            if (employee is null)
                return Result.Failure<AddApplicationForChangeOfPositionResponse>(EmployeeErrors.NotFound);

            var currentJobId = employee.JobId;

            // Validate new job exists
            var newJob = await _jobRepository.GetByIdDefaultAsync(
                request.NewJobId,
                cancellationToken);
            if (newJob is null)
                return Result.Failure<AddApplicationForChangeOfPositionResponse>(
                    ApplicationForChangeOfPositionErrors.JobNotFound);

            // Validate positions are different
            if (currentJobId == request.NewJobId)
                return Result.Failure<AddApplicationForChangeOfPositionResponse>(
                    ApplicationForChangeOfPositionErrors.SamePosition);

            var application = ApplicationForChangeOfPosition.Create(
                request.EmployeeId,
                currentJobId,
                request.NewJobId,
                request.SetDate,
                _userContext.UserId);

            await _applicationRepository.AddAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var saved = await _applicationRepository.GetByIdDefaultAsync(application.Id, cancellationToken);
            return Result.Success(_mapper.Map<AddApplicationForChangeOfPositionResponse>(saved));
        }
        catch (Exception)
        {
            return Result.Failure<AddApplicationForChangeOfPositionResponse>(
                ApplicationForChangeOfPositionErrors.AddFailed);
        }
    }
}
