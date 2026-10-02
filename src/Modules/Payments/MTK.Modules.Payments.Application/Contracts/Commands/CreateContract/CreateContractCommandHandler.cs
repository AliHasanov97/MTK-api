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
    private readonly IUnitOfWork _unitOfWork;

    public CreateContractCommandHandler(
        IContractRepository contractRepository,
        IVendorRepository vendorRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _vendorRepository = vendorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);

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
            request.VendorId,
            request.StartDate,
            request.EndDate,
            request.CreatedByUserId,
            request.Note);

        _contractRepository.Add(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(contract.Id);
    }
}
