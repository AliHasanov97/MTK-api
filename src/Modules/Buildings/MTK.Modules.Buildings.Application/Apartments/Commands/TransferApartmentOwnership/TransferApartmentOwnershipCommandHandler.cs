using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.OwnershipHistories;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.OwnershipHistories;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.TransferApartmentOwnership;

internal sealed class TransferApartmentOwnershipCommandHandler
    : ICommandHandler<TransferApartmentOwnershipCommand>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IOwnershipHistoryRepository _ownershipHistoryRepository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public TransferApartmentOwnershipCommandHandler(
        IApartmentRepository apartmentRepository,
        IOwnerRepository ownerRepository,
        IOwnershipHistoryRepository ownershipHistoryRepository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _apartmentRepository = apartmentRepository;
        _ownerRepository = ownerRepository;
        _ownershipHistoryRepository = ownershipHistoryRepository;
        _eventBus = eventBus;
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

        // Transfer date ayın 1-nə düzəlt
        var transferDate = new DateTime(
            request.TransferDate.Year,
            request.TransferDate.Month,
            1,
            0, 0, 0,
            DateTimeKind.Utc);

        // OwnershipHistory yarat
        var ownershipHistory = OwnershipHistory.Create(
            request.ApartmentId,
            previousOwnerId,
            previousOwnerName,
            request.NewOwnerId,
            newOwner.FullName,
            transferDate,
            request.SalePrice,
            request.Notes);

        _ownershipHistoryRepository.Add(ownershipHistory);

        // Apartment owner-ini yenilə
        apartment.MarkInTransfer();
        apartment.AssignOwner(request.NewOwnerId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Integration event - Billing module consume edəcək
        var integrationEvent = new OwnershipTransferredIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            request.ApartmentId,
            previousOwnerId,
            request.NewOwnerId,
            transferDate);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        return Result.Success();
    }
}
