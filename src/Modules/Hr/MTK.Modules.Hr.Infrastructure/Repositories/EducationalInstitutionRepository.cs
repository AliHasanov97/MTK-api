using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.EducationalInstitutions;
using MTK.Modules.Hr.Infrastructure.Database;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class EducationalInstitutionRepository : SearchableRepository<EducationalInstitution>, IEducationalInstitutionRepository
{
    public EducationalInstitutionRepository(HrDbContext context) : base(context)
    {
    }
}
