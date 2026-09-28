using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class PropertyOwnershipRepository : Repository<PropertyOwnership>, IPropertyOwnershipRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public PropertyOwnershipRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PropertyOwnership?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.PropertyOwnerships
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId, cancellationToken);
    }

    public async Task<List<PropertyOwnership>> GetByPropertyTypeAsync(PropertyType propertyType, CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.PropertyOwnerships
            .Where(p => p.PropertyType == propertyType)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PropertyOwnership>> GetAllWithOwnersAsync(CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.PropertyOwnerships
            .ToListAsync(cancellationToken);
    }
}
