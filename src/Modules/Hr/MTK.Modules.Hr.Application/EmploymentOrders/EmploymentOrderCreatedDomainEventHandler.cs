using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmploymentOrders.Events;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Application.EmploymentOrders;

internal sealed class EmploymentOrderCreatedDomainEventHandler
    : DomainEventHandler<EmploymentOrderCreatedDomainEvent>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EmploymentOrderCreatedDomainEventHandler(
        IEmployeeRepository employeeRepository,
        IJobApplicationRepository jobApplicationRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _jobApplicationRepository = jobApplicationRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task Handle(
        EmploymentOrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var jobApplication = await _jobApplicationRepository.GetByIdDefaultAsync(domainEvent.JobApplicationId, cancellationToken);
        var registerNumber = await _employeeRepository.GetNextRegisterNumberAsync(cancellationToken);

        var employee = Employee.Create(
            registerNumber: registerNumber,
            name: domainEvent.Name,
            surname: domainEvent.Surname,
            fathersName: domainEvent.FathersName,
            gender: domainEvent.Gender,
            startWorkDate: domainEvent.OrderDate,
            jobId: domainEvent.JobId,
            createdById: domainEvent.CreatedById,
            employmentOrderId: domainEvent.EmploymentOrderId,
            phoneNumber: jobApplication?.Telephone,
            homePhoneNumber: jobApplication?.HomeTelephoneNumber,
            currentAddress: jobApplication?.Address);

        _employeeRepository.Add(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

