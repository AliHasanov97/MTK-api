using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.UnpaidLeaveApplications;

public interface IUnpaidLeaveApplicationRepository : IRepository<UnpaidLeaveApplication>
{
    /// <summary>
    /// ID ilə unpaid leave application götür
    /// </summary>
    Task<UnpaidLeaveApplication?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin bütün ödənişsiz məzuniyyət müraciətlərini götür
    /// </summary>
    Task<List<UnpaidLeaveApplication>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}