using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.EducationLeaveOrders;

public static class EducationLeaveOrderErrors
{
    public static Error NotFound => new("EducationLeaveOrderNotFound", "Təhsil məzuniyyəti əmri tapılmadı.");
}
