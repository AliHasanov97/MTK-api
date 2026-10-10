using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Employees.Events;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;

namespace MTK.Modules.Hr.Application.EmployeeWorkSchedules;

/// <summary>
/// Employee yaradıldıqda default iş qrafiki yaradır (BE-C, 8 saat)
/// </summary>
internal sealed class EmployeeCreatedWorkScheduleHandler
    : DomainEventHandler<EmployeeCreatedDomainEvent>
{
    private readonly IEmployeeWorkScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeCreatedWorkScheduleHandler(
        IEmployeeWorkScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task Handle(
        EmployeeCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // Default iş qrafiki: boş (UI-dan doldurulacaq)
        // EffectiveFrom = işə başlama tarixi (keçmişə aid olsun)
        var effectiveFrom = DateOnly.FromDateTime(domainEvent.StartWorkDate.Date);

        var schedule = EmployeeWorkSchedule.Create(
            employeeId: domainEvent.EmployeeId,
            effectiveFrom: effectiveFrom,
            monday: null,
            tuesday: null,
            wednesday: null,
            thursday: null,
            friday: null,
            saturday: null,
            sunday: null);

        await _scheduleRepository.AddAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
