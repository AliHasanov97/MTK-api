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
            .Include(p => p.Apartment).ThenInclude(a => a!.Building)
            .Include(p => p.Garage)
            .FirstOrDefaultAsync(p => p.ApartmentId == propertyId || p.GarageId == propertyId, cancellationToken);
    }

    public async Task<List<PropertyOwnership>> GetByPropertyTypeAsync(PropertyType propertyType, CancellationToken cancellationToken = default)
    {
        var query = PaymentsContext.PropertyOwnerships
            .Include(p => p.Apartment).ThenInclude(a => a!.Building)
            .Include(p => p.Garage)
            .Include(p => p.Owner)
            .AsQueryable();

        query = propertyType == PropertyType.Apartment
            ? query.Where(p => p.ApartmentId != null)
            : query.Where(p => p.GarageId != null);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<PropertyOwnership>> GetAllWithOwnersAsync(CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.PropertyOwnerships
            .Include(p => p.Apartment).ThenInclude(a => a!.Building)
            .Include(p => p.Garage)
            .Include(p => p.Owner)
            .ToListAsync(cancellationToken);
    }
}
