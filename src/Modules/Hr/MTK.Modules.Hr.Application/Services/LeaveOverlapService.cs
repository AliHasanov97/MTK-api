using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;
using MTK.Modules.Hr.Domain.VacationApplications;
using MTK.Modules.Hr.Domain.VacationOrders;
using MTK.Modules.Hr.Domain.VacationReturnOrders;

namespace MTK.Modules.Hr.Application.Services;

/// <summary>
/// Məzuniyyət balansı izlənmədiyi üçün işçinin məzuniyyət/ödənişsiz/təhsil məzuniyyətlərinin
/// bir-biri ilə üst-üstə düşməməsini və geri çağırmanın real məzuniyyətə aid olmasını yoxlayır.
/// </summary>
public interface ILeaveOverlapService
{
    /// <param name="excludeApplicationId">Redaktə olunan ərizə (özü ilə toqquşmasın)</param>
    Task<Result> EnsureNoOverlapAsync(
        Guid employeeId,
        DateTimeOffset start,
        DateTimeOffset end,
        Guid? excludeApplicationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>Geri çağırma tarixi işçinin məzuniyyət aralığına düşməlidir və həmin məzuniyyət üçün təkrar olmamalıdır.</summary>
    Task<Result> EnsureReturnDateIsValidAsync(
        Guid employeeId,
        DateTimeOffset returnDate,
        CancellationToken cancellationToken = default);
}

internal sealed class LeaveOverlapService : ILeaveOverlapService
{
    private readonly IVacationOrderRepository _vacationOrders;
    private readonly IUnpaidLeaveOrderRepository _unpaidOrders;
    private readonly IEducationLeaveOrderRepository _educationOrders;
    private readonly IVacationApplicationRepository _vacationApplications;
    private readonly IUnpaidLeaveApplicationRepository _unpaidApplications;
    private readonly IEducationLeaveApplicationRepository _educationApplications;
    private readonly IVacationReturnOrderRepository _returnOrders;

    public LeaveOverlapService(
        IVacationOrderRepository vacationOrders,
        IUnpaidLeaveOrderRepository unpaidOrders,
        IEducationLeaveOrderRepository educationOrders,
        IVacationApplicationRepository vacationApplications,
        IUnpaidLeaveApplicationRepository unpaidApplications,
        IEducationLeaveApplicationRepository educationApplications,
        IVacationReturnOrderRepository returnOrders)
    {
        _vacationOrders = vacationOrders;
        _unpaidOrders = unpaidOrders;
        _educationOrders = educationOrders;
        _vacationApplications = vacationApplications;
        _unpaidApplications = unpaidApplications;
        _educationApplications = educationApplications;
        _returnOrders = returnOrders;
    }

    private static DateTimeOffset Day(DateTimeOffset d) => new(d.UtcDateTime.Date, TimeSpan.Zero);

    private static Error Overlap(string kind, DateTimeOffset from, DateTimeOffset to) => new(
        "Leave.Overlap",
        $"İşçinin {from:dd.MM.yyyy} - {to:dd.MM.yyyy} tarixləri arasında artıq {kind} var. Tarixlər üst-üstə düşə bilməz");

    public async Task<Result> EnsureNoOverlapAsync(
        Guid employeeId,
        DateTimeOffset start,
        DateTimeOffset end,
        Guid? excludeApplicationId = null,
        CancellationToken cancellationToken = default)
    {
        var s = Day(start);
        var e = Day(end);

        // Əmrlər. Geri çağırma olubsa, məzuniyyət geri çağırma tarixindən əvvəlki gündə bitmiş sayılır.
        var vacations = await _vacationOrders.ListAsync(
            o => o.EmployeeId == employeeId && o.StartDate <= e && o.EndDate >= s, cancellationToken);
        if (vacations.Count > 0)
        {
            var returns = await _returnOrders.ListAsync(o => o.EmployeeId == employeeId, cancellationToken);
            foreach (var v in vacations)
            {
                var effectiveEnd = Day(v.EndDate);
                var ret = returns
                    .Where(r => Day(r.ReturnDate) >= Day(v.StartDate) && Day(r.ReturnDate) <= Day(v.EndDate))
                    .Select(r => Day(r.ReturnDate))
                    .DefaultIfEmpty(DateTimeOffset.MaxValue)
                    .Min();
                if (ret != DateTimeOffset.MaxValue)
                    effectiveEnd = ret.AddDays(-1);

                if (Day(v.StartDate) <= e && effectiveEnd >= s)
                    return Result.Failure(Overlap("əmək məzuniyyəti", v.StartDate, effectiveEnd));
            }
        }

        var unpaid = await _unpaidOrders.ListAsync(
            o => o.EmployeeId == employeeId && o.StartDate <= e && o.EndDate >= s, cancellationToken);
        if (unpaid.Count > 0)
            return Result.Failure(Overlap("ödənişsiz məzuniyyət", unpaid[0].StartDate, unpaid[0].EndDate));

        var education = await _educationOrders.ListAsync(
            o => o.EmployeeId == employeeId && o.StartDate <= e && o.EndDate >= s, cancellationToken);
        if (education.Count > 0)
            return Result.Failure(Overlap("təhsil məzuniyyəti", education[0].StartDate, education[0].EndDate));

        // Hələ əmrə çevrilməmiş ərizələr
        var pendingVacation = await _vacationApplications.ListAsync(
            a => a.EmployeeId == employeeId && a.Status == ApplicationStatus.PendingApproval &&
                 a.Id != excludeApplicationId && a.StartDate <= e && a.EndDate != null && a.EndDate >= s,
            cancellationToken);
        if (pendingVacation.Count > 0)
            return Result.Failure(Overlap("əmək məzuniyyəti ərizəsi", pendingVacation[0].StartDate, pendingVacation[0].EndDate!.Value));

        var pendingUnpaid = await _unpaidApplications.ListAsync(
            a => a.EmployeeId == employeeId && a.Status == ApplicationStatus.PendingApproval &&
                 a.Id != excludeApplicationId && a.StartDate <= e && a.EndDate >= s,
            cancellationToken);
        if (pendingUnpaid.Count > 0)
            return Result.Failure(Overlap("ödənişsiz məzuniyyət ərizəsi", pendingUnpaid[0].StartDate, pendingUnpaid[0].EndDate));

        var pendingEducation = await _educationApplications.ListAsync(
            a => a.EmployeeId == employeeId && a.Status == ApplicationStatus.PendingApproval &&
                 a.Id != excludeApplicationId && a.StartDate <= e && a.EndDate >= s,
            cancellationToken);
        if (pendingEducation.Count > 0)
            return Result.Failure(Overlap("təhsil məzuniyyəti ərizəsi", pendingEducation[0].StartDate, pendingEducation[0].EndDate));

        return Result.Success();
    }

    public async Task<Result> EnsureReturnDateIsValidAsync(
        Guid employeeId,
        DateTimeOffset returnDate,
        CancellationToken cancellationToken = default)
    {
        var d = Day(returnDate);

        var vacation = (await _vacationOrders.ListAsync(
                o => o.EmployeeId == employeeId && o.StartDate <= d && o.EndDate >= d, cancellationToken))
            .FirstOrDefault();

        if (vacation is null)
            return Result.Failure(new Error(
                "VacationReturn.NotOnVacation",
                $"İşçi {d:dd.MM.yyyy} tarixində əmək məzuniyyətində deyil. Geri çağırma yalnız məzuniyyət müddətinə aid ola bilər"));

        var existing = await _returnOrders.ListAsync(
            r => r.EmployeeId == employeeId && r.ReturnDate >= vacation.StartDate && r.ReturnDate <= vacation.EndDate,
            cancellationToken);
        if (existing.Count > 0)
            return Result.Failure(new Error(
                "VacationReturn.AlreadyReturned",
                $"Bu məzuniyyət üçün artıq geri çağırma əmri var ({existing[0].ReturnDate:dd.MM.yyyy})"));

        return Result.Success();
    }
}
