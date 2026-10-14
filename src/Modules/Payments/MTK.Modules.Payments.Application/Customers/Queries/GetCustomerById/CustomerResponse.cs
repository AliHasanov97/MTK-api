using MTK.Modules.Payments.Domain.Customers;

namespace MTK.Modules.Payments.Application.Customers.Queries.GetCustomerById;

public sealed record CustomerResponse(
    Guid Id,
    string Name,
    CustomerType CustomerType,
    string? Voen,
    string? Director,
    string? Email,
    string? Phone,
    string? Address,
    string? Note,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
