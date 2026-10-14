using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Contracts.Commands.CreateContract;

internal sealed class CreateContractCommandHandler : ICommandHandler<CreateContractCommand, Guid>
{
    private readonly IContractRepository _contractRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContractCommandHandler(
        IContractRepository contractRepository,
        IVendorRepository vendorRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _vendorRepository = vendorRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        if (request.CustomerId is { } customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);

            if (customer is null)
            {
                return Result.Failure<Guid>(new Error(
                    "Customer.NotFound",
                    $"Müştəri tapılmadı: {customerId}"));
            }

            if (!customer.IsActive)
            {
                return Result.Failure<Guid>(new Error(
                    "Customer.Inactive",
                    $"Dayandırılmış müştəri ilə müqavilə bağlana bilməz: {customer.Name}"));
            }

            var customerContract = Contract.CreateForCustomer(
                request.Number,
                customerId,
                request.StartDate,
                request.EndDate,
                request.CreatedByUserId,
                request.Note);

            _contractRepository.Add(customerContract);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(customerContract.Id);
        }

        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId!.Value, cancellationToken);

        if (vendor is null)
        {
            return Result.Failure<Guid>(new Error(
                "Vendor.NotFound",
                $"Tədarükçü tapılmadı: {request.VendorId}"));
        }

        if (!vendor.IsActive)
        {
            return Result.Failure<Guid>(new Error(
                "Vendor.Inactive",
                $"Dayandırılmış tədarükçü ilə müqavilə bağlana bilməz: {vendor.Name}"));
        }

        // Nömrə unikallığı bazada (Contracts.Number üzrə unique index) qorunur.
        var contract = Contract.Create(
            request.Number,
            request.VendorId!.Value,
            request.StartDate,
            request.EndDate,
            request.CreatedByUserId,
            request.Note);

        _contractRepository.Add(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(contract.Id);
    }
}
