using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Vendors;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class VendorRepository : SearchableRepository<Vendor>, IVendorRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public VendorRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Vendor>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Vendors
            .Where(v => v.IsActive)
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsVoenUniqueAsync(string voen, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return !await PaymentsContext.Vendors
            .AnyAsync(v => v.Voen == voen && (excludeId == null || v.Id != excludeId), cancellationToken);
    }

    public Task<bool> HasActiveContractsAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Contracts
            .AnyAsync(c => c.VendorId == vendorId && c.Status == ContractStatus.Active, cancellationToken);
    }
}
