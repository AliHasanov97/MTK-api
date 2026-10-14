using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Employees;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }
}
