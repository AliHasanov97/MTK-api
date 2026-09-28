using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.PropertyOwnerships;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IPropertyOwnershipRepository : IRepository<PropertyOwnership>
{
    Task<PropertyOwnership?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<List<PropertyOwnership>> GetByPropertyTypeAsync(PropertyType propertyType, CancellationToken cancellationToken = default);
    Task<List<PropertyOwnership>> GetAllWithOwnersAsync(CancellationToken cancellationToken = default);
}
