using MTK.Common.Application.EventBus;
using MTK.Modules.Hr.IntegrationEvents.VacationPays;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Services.AverageWage;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.VacationPays;

namespace MTK.Modules.Payments.Presentation.Payroll;

/// <summary>
/// HR modulundan VacationOrder yaradıldıqda gələn integration event-i handle edir.
/// VacationPay entity-sini yaradır (Azərbaycan Əmək Məcəlləsinə uyğun orta əmək haqqı hesablaması ilə).
/// </summary>
internal sealed class VacationPayCreatedIntegrationEventHandler(
    IAverageWageCalculator averageWageCalculator,
    IVacationPayRepository vacationPayRepository,
    IUnitOfWork unitOfWork)
    : IntegrationEventHandler<VacationPayCreatedIntegrationEvent>
{
    public override async Task Handle(
        VacationPayCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        // 1. Idempotency yoxlaması - eyni VacationOrder üçün təkrar yaradılmasın
        var existingVacationPay = await vacationPayRepository.GetByVacationOrderIdAsync(
            integrationEvent.VacationOrderId,
            cancellationToken);

        if (existingVacationPay is not null)
        {
            // Artıq yaradılıb, təkrar emal etməyə ehtiyac yoxdur
            return;
        }

        // 2. Orta əmək haqqını hesabla (son 12 ay)
        var averageWageResult = await averageWageCalculator.CalculateAsync(
            integrationEvent.EmployeeId,
            integrationEvent.VacationStartDate.UtcDateTime,
            cancellationToken);

        // 3. Məzuniyyət haqqını hesabla
        var totalAmount = averageWageResult.AverageDailyWage * integrationEvent.VacationDays;

        // 4. DueDate hesabla (məzuniyyətdən 3 gün əvvəl)
        var vacationStartDate = DateOnly.FromDateTime(integrationEvent.VacationStartDate.UtcDateTime);
        var dueDate = vacationStartDate.AddDays(-3);

        // 5. VacationPay entity yarad
        var vacationPay = VacationPay.Create(
            integrationEvent.VacationOrderId,
            integrationEvent.EmployeeId,
            integrationEvent.EmployeeFullName,
            averageWageResult.AverageMonthlyWage,
            averageWageResult.AverageDailyWage,
            integrationEvent.VacationDays,
            totalAmount,
            vacationStartDate,
            DateOnly.FromDateTime(integrationEvent.VacationEndDate.UtcDateTime),
            dueDate);

        // 6. Saxla
        await vacationPayRepository.AddAsync(vacationPay, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
