using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.OwnershipHistories;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.TransferApartmentOwnership;

internal sealed class TransferApartmentOwnershipCommandHandler
    : ICommandHandler<TransferApartmentOwnershipCommand>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IOwnershipHistoryRepository _ownershipHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransferApartmentOwnershipCommandHandler(
        IApartmentRepository apartmentRepository,
        IOwnerRepository ownerRepository,
        IOwnershipHistoryRepository ownershipHistoryRepository,
        IUnitOfWork unitOfWork)
    {
        _apartmentRepository = apartmentRepository;
        _ownerRepository = ownerRepository;
        _ownershipHistoryRepository = ownershipHistoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        TransferApartmentOwnershipCommand request,
        CancellationToken cancellationToken)
    {
        var apartment = await _apartmentRepository.GetByIdAsync(
            request.ApartmentId,
            cancellationToken);

        if (apartment is null)
        {
            return Result.Failure(new Error(
                "Apartment.NotFound",
                $"Mənzil tapılmadı: {request.ApartmentId}"));
        }

        var newOwner = await _ownerRepository.GetByIdAsync(
            request.NewOwnerId,
            cancellationToken);

        if (newOwner is null)
        {
            return Result.Failure(new Error(
                "Owner.NotFound",
                $"Yeni sahib tapılmadı: {request.NewOwnerId}"));
        }

        // Köhnə owner məlumatları (snapshot)
        var previousOwnerId = apartment.CurrentOwnerId;
        string? previousOwnerName = null;

        if (previousOwnerId.HasValue)
        {
            var previousOwner = await _ownerRepository.GetByIdAsync(
                previousOwnerId.Value,
                cancellationToken);
            previousOwnerName = previousOwner?.FullName;
        }

        // Transfer tarixi həmişə indidir, ayın 1-nə düzəldilir.
        var now = DateTime.UtcNow;
        var transferDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        // OwnershipHistory yarat
        var ownershipHistory = OwnershipHistory.Create(
            request.ApartmentId,
            previousOwnerId,
            previousOwnerName,
            request.NewOwnerId,
            newOwner.FullName,
            transferDate);

        _ownershipHistoryRepository.Add(ownershipHistory);

        // Apartment owner-ini yenilə
        apartment.MarkInTransfer();
        apartment.AssignOwner(request.NewOwnerId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Domain event handler will publish integration event

        return Result.Success();
    }
}
