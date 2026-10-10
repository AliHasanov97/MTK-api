using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.CompensationOrders;

public static class CompensationOrderErrors
{
    public static Error NotFound(Guid id) => new Error(
        "CompensationOrder.NotFound",
        $"Kompensasiya əmri ID {id} ilə tapılmadı");

    public static readonly Error DeleteFailed = new Error(
        "CompensationOrder.DeleteFailed",
        "Kompensasiya əmri silinə bilmədi");
}