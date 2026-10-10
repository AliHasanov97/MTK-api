using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.CalendarDays;

public static class CalendarDayErrors
{
    public static Error NotFound => new("CalendarDay.NotFound", "Təqvim günü tapılmadı");
    public static Error DuplicateDate => new("CalendarDay.DuplicateDate", "Bu tarixdə təqvim günü artıq mövcuddur");
    public static Error EmptyName => new("CalendarDay.EmptyName", "Ad boş ola bilməz");
}
