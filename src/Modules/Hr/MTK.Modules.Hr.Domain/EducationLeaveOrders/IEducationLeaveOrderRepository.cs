using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EducationLeaveOrders;

public interface IEducationLeaveOrderRepository : IRepository<EducationLeaveOrder>
{
    Task<EducationLeaveOrder?> GetByEducationLeaveApplicationIdAsync(
        Guid educationLeaveApplicationId,
        CancellationToken cancellationToken = default);

    Task<EducationLeaveOrder?> GetByIdWithEmployeeAndCompanyAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}