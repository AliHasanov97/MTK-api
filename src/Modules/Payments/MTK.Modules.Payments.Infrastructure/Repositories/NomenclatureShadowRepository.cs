using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Nomenclatures;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class NomenclatureShadowRepository : SearchableRepository<NomenclatureShadow>, INomenclatureShadowRepository
{
    public NomenclatureShadowRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }
}
