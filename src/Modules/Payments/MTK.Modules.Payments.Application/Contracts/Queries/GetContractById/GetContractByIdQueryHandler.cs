using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Contracts.Queries.GetContractById;

internal sealed class GetContractByIdQueryHandler : IQueryHandler<GetContractByIdQuery, ContractResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IVendorRepository _vendorRepository;

    public GetContractByIdQueryHandler(
        IContractRepository contractRepository,
        IVendorRepository vendorRepository)
    {
        _contractRepository = contractRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<Result<ContractResponse>> Handle(
        GetContractByIdQuery request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetWithServicesAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure<ContractResponse>(new Error(
                "Contract.NotFound",
                $"Müqavilə tapılmadı: {request.ContractId}"));
        }

        var vendor = await _vendorRepository.GetByIdAsync(contract.VendorId, cancellationToken);

        var services = contract.Services
            .Select(s => new ContractServiceResponse(
                s.Id,
                s.Name,
                s.Description,
                s.Unit,
                s.UnitPrice,
                s.Quantity,
                s.BillingPeriod,
                s.PeriodAmount,
                s.ServiceStartDate,
                s.ServiceEndDate,
                s.PaymentTermDays,
                s.IsActive))
            .ToList();

        var response = new ContractResponse(
            contract.Id,
            contract.Number,
            contract.VendorId,
            vendor?.Name,
            contract.CreatedByUserId,
            contract.StartDate,
            contract.EndDate,
            contract.Status,
            contract.Currency,
            contract.Note,
            contract.IsExpired,
            contract.IsActive,
            contract.MonthlyAmount,
            services.Sum(s => s.PeriodAmount),
            contract.CreatedAt,
            contract.UpdatedAt,
            services);

        return response;
    }
}
