using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Customers;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class CustomerRepository : SearchableRepository<Customer>, ICustomerRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public CustomerRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsVoenUniqueAsync(string voen, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return !await PaymentsContext.Customers
            .AnyAsync(c => c.Voen == voen && (excludeId == null || c.Id != excludeId), cancellationToken);
    }

    public Task<bool> HasActiveContractsAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Contracts
            .AnyAsync(c => c.CustomerId == customerId && c.Status == ContractStatus.Active, cancellationToken);
    }

    public Task<bool> HasOpenChargesAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Charges
            .AnyAsync(c =>
                c.CustomerId == customerId &&
                c.Status != ChargeStatus.Paid &&
                c.Status != ChargeStatus.Cancelled,
                cancellationToken);
    }
}
