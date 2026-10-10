using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EducationLeaveApplications;

public interface IEducationLeaveApplicationRepository : IRepository<EducationLeaveApplication>
{
    /// <summary>
    /// ID ilə education leave application götür
    /// </summary>
    Task<EducationLeaveApplication?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin bütün təhsil məzuniyyəti müraciətlərini götür
    /// </summary>
    Task<List<EducationLeaveApplication>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}