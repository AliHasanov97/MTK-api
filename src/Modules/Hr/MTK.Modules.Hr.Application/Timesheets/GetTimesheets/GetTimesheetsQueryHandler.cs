using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;
using MTK.Modules.Hr.Domain.VacationOrders;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using MTK.Modules.Hr.Domain.VacationReturnOrders;

namespace MTK.Modules.Hr.Application.Timesheets.GetTimesheets;

internal sealed class GetTimesheetsQueryHandler : IQueryHandler<GetTimesheetsQuery, GetTimesheetsResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly IVacationOrderRepository _vacationOrderRepository;
    private readonly IUnpaidLeaveOrderRepository _unpaidLeaveOrderRepository;
    private readonly IEducationLeaveOrderRepository _educationLeaveOrderRepository;
    private readonly IVacationReturnApplicationRepository _vacationReturnApplicationRepository;
    private readonly IVacationReturnOrderRepository _vacationReturnOrderRepository;
    private readonly IUnexcusedAbsenceRepository _unexcusedAbsenceRepository;

    public GetTimesheetsQueryHandler(
        IEmployeeRepository employeeRepository,
        ICalendarDayRepository calendarDayRepository,
        IVacationOrderRepository vacationOrderRepository,
        IUnpaidLeaveOrderRepository unpaidLeaveOrderRepository,
        IEducationLeaveOrderRepository educationLeaveOrderRepository,
        IVacationReturnApplicationRepository vacationReturnApplicationRepository,
        IVacationReturnOrderRepository vacationReturnOrderRepository,
        IUnexcusedAbsenceRepository unexcusedAbsenceRepository)
    {
        _employeeRepository = employeeRepository;
        _calendarDayRepository = calendarDayRepository;
        _vacationOrderRepository = vacationOrderRepository;
        _unpaidLeaveOrderRepository = unpaidLeaveOrderRepository;
        _educationLeaveOrderRepository = educationLeaveOrderRepository;
        _vacationReturnApplicationRepository = vacationReturnApplicationRepository;
        _vacationReturnOrderRepository = vacationReturnOrderRepository;
        _unexcusedAbsenceRepository = unexcusedAbsenceRepository;
    }

    public async Task<Result<GetTimesheetsResponse>> Handle(
        GetTimesheetsQuery request,
        CancellationToken cancellationToken)
    {
        var startDate = new DateOnly(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);

        // İşçiləri al - Vəzifə ilə birlikdə
        // Filter: O ayda işləyən işçilər (StartWorkDate <= ayın sonu)
        var employees = await _employeeRepository.GetForTimesheetAsync(
            endDate, cancellationToken);

        if (employees.Count == 0)
        {
            return Result.Success(new GetTimesheetsResponse
            {
                Year = request.Year,
                Month = request.Month,
                DaysInMonth = daysInMonth,
                Employees = new List<EmployeeTimesheetDto>()
            });
        }

        // Təqvim günlərini al
        var calendarDays = await _calendarDayRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);

        // İşçi ID-lərini al
        var employeeIds = employees.Select(e => e.Id).ToList();

        // Əmrləri al
        var vacationOrders = await GetOrdersForMonth(_vacationOrderRepository, employeeIds, startDate, endDate, cancellationToken);
        var unpaidLeaveOrders = await GetOrdersForMonth(_unpaidLeaveOrderRepository, employeeIds, startDate, endDate, cancellationToken);
        var educationLeaveOrders = await GetOrdersForMonth(_educationLeaveOrderRepository, employeeIds, startDate, endDate, cancellationToken);

        // Məzuniyyətdən geri çağırılma əmrlərini al
        var vacationReturnOrders = await GetVacationReturnOrdersForMonth(employeeIds, startDate, endDate, cancellationToken);

        // Üzrsüz səbəbdən işə gəlməmə əmrləri (işçi -> tarixlər)
        var absences = await GetUnexcusedAbsencesForMonth(employeeIds, startDate, endDate, cancellationToken);

        // Hər işçi üçün timesheet hesabla
        var employeeTimesheets = new List<EmployeeTimesheetDto>();

        foreach (var employee in employees)
        {
            var employeeStartDate = DateOnly.FromDateTime(employee.StartWorkDate.Date);

            // Təqvim günlərindən yalnız bu işçinin iş qrafikinə (5/6 günlük) tətbiq olunanlar
            var employeeWorkingDays = employee.WorkingDays ?? WorkingDays.FiveDays;
            var calendarDayMap = calendarDays
                .Where(c => c.ApplicableWorkingDays == null || c.ApplicableWorkingDays == employeeWorkingDays)
                .GroupBy(c => c.Date)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(c => c.ApplicableWorkingDays.HasValue).First());

            var days = new List<TimesheetDayDto>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateOnly(request.Year, request.Month, day);

                // İşçi hələ işə başlamamışdı
                if (date < employeeStartDate)
                {
                    days.Add(new TimesheetDayDto
                    {
                        Date = date,
                        Day = day,
                        DayType = "NotEmployed",
                        DayCode = "-",
                        WorkedHours = null
                    });
                    continue;
                }

                // Hər tarix üçün o tarixdə aktiv olan qrafiki tap
                var schedule = employee.GetScheduleForDate(date);

                var (dayType, dayCode, workedHours) = CalculateDayType(
                    employee, date, schedule, calendarDayMap,
                    vacationOrders, unpaidLeaveOrders, educationLeaveOrders, vacationReturnOrders);

                // Üzrsüz işə gəlməmə: yalnız həmin gün işçi işləməli idisə (iş günü) tətbiq olunur;
                // məzuniyyət, bayram və istirahət günləri dəyişmir
                if (dayCode == "İ" && absences.TryGetValue(employee.Id, out var absentDates) && absentDates.Contains(date))
                {
                    dayType = "UnexcusedAbsence";
                    dayCode = "İG";
                    workedHours = null;
                }

                days.Add(new TimesheetDayDto
                {
                    Date = date,
                    Day = day,
                    DayType = dayType,
                    DayCode = dayCode,
                    WorkedHours = workedHours
                });
            }

            // Norma hesabla (işçinin qrafikinə görə bu ayda neçə gün/saat işləməli idi)
            var (normDays, normHours) = CalculateNorm(employee, startDate, endDate, calendarDayMap, employeeStartDate);

            employeeTimesheets.Add(new EmployeeTimesheetDto
            {
                Employee = ResponseObjectWithName.Create(
                    employee.Id,
                    $"{employee.Surname} {employee.Name} {employee.FathersName}"),
                Position = employee.Job != null
                    ? ResponseObjectWithName.Create(employee.Job.Id, employee.Job.Name)
                    : null,
                RegisterNumber = employee.RegisterNumber,
                Days = days,

                // Faktiki
                ActualWorkingDays = days.Count(d => d.DayCode == "İ"),
                ActualWorkedHours = days.Where(d => d.WorkedHours.HasValue).Sum(d => d.WorkedHours!.Value),

                // Norma
                NormWorkingDays = normDays,
                NormWorkedHours = normHours,

                // Məzuniyyət və digər günlər
                AnnualLeaveDays = days.Count(d => d.DayCode == "M"),
                SocialLeaveDays = days.Count(d => d.DayCode == "Y"),
                EducationLeaveDays = days.Count(d => d.DayCode == "TM"),
                UnpaidLeaveDays = days.Count(d => d.DayCode == "ÖM"),
                SickLeaveDays = days.Count(d => d.DayCode == "X"),
                BusinessTripDays = days.Count(d => d.DayCode == "E"),
                HolidayDays = days.Count(d => d.DayCode == "B" || d.DayCode == "H"),
                NonWorkingDays = days.Count(d => d.DayCode == "Q"),
                UnexcusedAbsenceDays = days.Count(d => d.DayCode == "İG")
            });
        }

        return Result.Success(new GetTimesheetsResponse
        {
            Year = request.Year,
            Month = request.Month,
            DaysInMonth = daysInMonth,
            Employees = employeeTimesheets
        });
    }

    /// <summary>
    /// İşçinin qrafikinə görə ay üçün norma hesablayır
    /// </summary>
    private (int NormDays, decimal NormHours) CalculateNorm(
        Employee employee,
        DateOnly startDate,
        DateOnly endDate,
        Dictionary<DateOnly, CalendarDay> calendarDayMap,
        DateOnly employeeStartDate)
    {
        int normDays = 0;
        decimal normHours = 0m;

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            // İşçi hələ işə başlamamışdı
            if (date < employeeStartDate)
                continue;

            // Hər tarix üçün o tarixdə aktiv olan qrafiki tap
            var schedule = employee.GetScheduleForDate(date);

            // Bayram günüdür
            if (calendarDayMap.TryGetValue(date, out var calendarDay))
            {
                if (calendarDay.DayType == CalendarDayType.Holiday ||
                    calendarDay.DayType == CalendarDayType.NonWorkingDay)
                    continue;

                // İş günü edilmiş gün
                if (calendarDay.DayType == CalendarDayType.WorkingDay)
                {
                    var hours = schedule?.GetWorkHours(date.DayOfWeek);
                    if (hours.HasValue && hours.Value > 0)
                    {
                        normDays++;
                        normHours += hours.Value;
                    }
                    continue;
                }
            }

            // İşçinin qrafikinə görə
            if (schedule != null)
            {
                var workHours = schedule.GetWorkHours(date.DayOfWeek);
                if (workHours.HasValue && workHours.Value > 0)
                {
                    normDays++;
                    normHours += workHours.Value;
                }
            }
            else
            {
                // Schedule yoxdursa, norma hesablanmır
                // İşçinin mütləq work schedule-u olmalıdır
            }
        }

        return (normDays, normHours);
    }

    private (string DayType, string DayCode, decimal? WorkedHours) CalculateDayType(
        Employee employee,
        DateOnly date,
        EmployeeWorkSchedule? schedule,
        Dictionary<DateOnly, CalendarDay> calendarDayMap,
        Dictionary<Guid, List<(DateOnly Start, DateOnly End)>> vacationOrders,
        Dictionary<Guid, List<(DateOnly Start, DateOnly End)>> unpaidLeaveOrders,
        Dictionary<Guid, List<(DateOnly Start, DateOnly End)>> educationLeaveOrders,
        Dictionary<Guid, List<DateOnly>> vacationReturnOrders)
    {
        // 1. Əmək məzuniyyəti (məzuniyyətdən geri çağırılmanı nəzərə al)
        if (vacationOrders.TryGetValue(employee.Id, out var vacations))
        {
            var vacation = vacations.FirstOrDefault(v => v.Start <= date && v.End >= date);
            if (vacation != default)
            {
                // Məzuniyyətdən geri çağırılma var mı?
                if (vacationReturnOrders.TryGetValue(employee.Id, out var returnDates))
                {
                    // Əgər geri qayıdış tarixi bu tarixdən əvvəl və ya bu tarixdədirsə,
                    // işçi artıq işə qayıdıb, ona görə məzuniyyət sayılmır
                    var returnDate = returnDates.FirstOrDefault(r => r <= date && r >= vacation.Start);
                    if (returnDate != default)
                    {
                        // Geri qayıdış tarixindən sonra - iş günü hesabla
                        goto CheckWorkingDay;
                    }
                }
                return ("AnnualLeave", "M", null);
            }
        }

        CheckWorkingDay:

        // 2. Ödənişsiz məzuniyyət
        if (unpaidLeaveOrders.TryGetValue(employee.Id, out var unpaidLeaves))
        {
            var unpaidLeave = unpaidLeaves.FirstOrDefault(u => u.Start <= date && u.End >= date);
            if (unpaidLeave != default)
                return ("UnpaidLeave", "ÖM", null);
        }

        // 3. Təhsil məzuniyyəti
        if (educationLeaveOrders.TryGetValue(employee.Id, out var eduLeaves))
        {
            var eduLeave = eduLeaves.FirstOrDefault(e => e.Start <= date && e.End >= date);
            if (eduLeave != default)
                return ("EducationLeave", "TM", null);
        }

        // 4. Təqvim günü (bayram)
        if (calendarDayMap.TryGetValue(date, out var calendarDay))
        {
            if (calendarDay.DayType == CalendarDayType.Holiday)
                return ("Holiday", "B", null);

            if (calendarDay.DayType == CalendarDayType.NonWorkingDay)
                return ("NonWorkingDay", "Q", null);

            if (calendarDay.DayType == CalendarDayType.WorkingDay)
            {
                var workHours = schedule?.GetWorkHours(date.DayOfWeek);
                return ("Working", "İ", workHours);
            }
        }

        // 5. İşçinin qrafikinə görə
        if (schedule != null)
        {
            var workHours = schedule.GetWorkHours(date.DayOfWeek);
            if (workHours.HasValue && workHours.Value > 0)
                return ("Working", "İ", workHours);
            else
                return ("NonWorkingDay", "Q", null);
        }

        // 6. Default: schedule yoxdursa, şənbə-bazar istirahət, həftə içi iş (saatsız)
        var dayOfWeek = date.DayOfWeek;
        if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
            return ("NonWorkingDay", "Q", null);

        // Schedule olmadan iş saatı bilinmir
        return ("Working", "İ", null);
    }

    private async Task<Dictionary<Guid, List<(DateOnly Start, DateOnly End)>>> GetOrdersForMonth<T>(
        IRepository<T> repository,
        List<Guid> employeeIds,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken) where T : Entity
    {
        var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
        var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

        var result = new Dictionary<Guid, List<(DateOnly Start, DateOnly End)>>();

        if (typeof(T) == typeof(VacationOrder))
        {
            var orders = await (_vacationOrderRepository as IRepository<VacationOrder>)!.ListAsync(
                o => employeeIds.Contains(o.EmployeeId) &&
                     o.StartDate <= endDateTime &&
                     o.EndDate >= startDateTime,
                cancellationToken);

            foreach (var order in orders)
            {
                if (!result.ContainsKey(order.EmployeeId))
                    result[order.EmployeeId] = new List<(DateOnly, DateOnly)>();

                result[order.EmployeeId].Add((
                    DateOnly.FromDateTime(order.StartDate.Date),
                    DateOnly.FromDateTime(order.EndDate.Date)));
            }
        }
        else if (typeof(T) == typeof(UnpaidLeaveOrder))
        {
            var orders = await (_unpaidLeaveOrderRepository as IRepository<UnpaidLeaveOrder>)!.ListAsync(
                o => employeeIds.Contains(o.EmployeeId) &&
                     o.StartDate <= endDateTime &&
                     o.EndDate >= startDateTime,
                cancellationToken);

            foreach (var order in orders)
            {
                if (!result.ContainsKey(order.EmployeeId))
                    result[order.EmployeeId] = new List<(DateOnly, DateOnly)>();

                result[order.EmployeeId].Add((
                    DateOnly.FromDateTime(order.StartDate.Date),
                    DateOnly.FromDateTime(order.EndDate.Date)));
            }
        }
        else if (typeof(T) == typeof(EducationLeaveOrder))
        {
            var orders = await (_educationLeaveOrderRepository as IRepository<EducationLeaveOrder>)!.ListAsync(
                o => employeeIds.Contains(o.EmployeeId) &&
                     o.StartDate <= endDateTime &&
                     o.EndDate >= startDateTime,
                cancellationToken);

            foreach (var order in orders)
            {
                if (!result.ContainsKey(order.EmployeeId))
                    result[order.EmployeeId] = new List<(DateOnly, DateOnly)>();

                result[order.EmployeeId].Add((
                    DateOnly.FromDateTime(order.StartDate.Date),
                    DateOnly.FromDateTime(order.EndDate.Date)));
            }
        }

        return result;
    }

    private async Task<Dictionary<Guid, HashSet<DateOnly>>> GetUnexcusedAbsencesForMonth(
        List<Guid> employeeIds,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        // SetDate UTC gün başlanğıcı kimi saxlanılır; sərhədləri bir gün genişləndirib tarixə görə süzürük
        var from = new DateTimeOffset(startDate.AddDays(-1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var to = new DateTimeOffset(endDate.AddDays(1).ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);

        var orders = await _unexcusedAbsenceRepository.ListAsync(
            o => employeeIds.Contains(o.EmployeeId) && o.SetDate >= from && o.SetDate <= to,
            cancellationToken);

        var result = new Dictionary<Guid, HashSet<DateOnly>>();
        foreach (var order in orders)
        {
            if (!result.TryGetValue(order.EmployeeId, out var set))
                result[order.EmployeeId] = set = new HashSet<DateOnly>();
            set.Add(DateOnly.FromDateTime(order.SetDate.UtcDateTime));
        }

        return result;
    }

    /// <summary>
    /// Məzuniyyətdən geri çağırılma əmrlərini alır
    /// </summary>
    private async Task<Dictionary<Guid, List<DateOnly>>> GetVacationReturnOrdersForMonth(
        List<Guid> employeeIds,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var startDateTime = new DateTimeOffset(startDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var endDateTime = new DateTimeOffset(endDate.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);

        var orders = await _vacationReturnOrderRepository.ListAsync(
            o => employeeIds.Contains(o.EmployeeId) &&
                 o.ReturnDate >= startDateTime &&
                 o.ReturnDate <= endDateTime,
            cancellationToken);

        var result = new Dictionary<Guid, List<DateOnly>>();

        foreach (var order in orders)
        {
            if (!result.ContainsKey(order.EmployeeId))
                result[order.EmployeeId] = new List<DateOnly>();

            result[order.EmployeeId].Add(DateOnly.FromDateTime(order.ReturnDate.Date));
        }

        return result;
    }
}
