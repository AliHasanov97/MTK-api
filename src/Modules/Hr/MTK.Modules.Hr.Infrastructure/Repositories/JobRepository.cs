using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.Jobs;
using MTK.Modules.Hr.Infrastructure.Database;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class JobRepository : SearchableRepository<Job>, IJobRepository
{
    public JobRepository(HrDbContext context) : base(context)
    {
    }
}
