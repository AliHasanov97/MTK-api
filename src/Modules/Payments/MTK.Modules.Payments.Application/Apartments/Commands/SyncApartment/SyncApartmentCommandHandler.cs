using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Apartments;
using MTK.Modules.Payments.Domain.Buildings;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Apartments.Commands.SyncApartment;

/// <summary>
/// Upserts both shadows in one go — Building first (Apartment's FK depends on it
/// existing), then Apartment. Both come from the same Buildings integration event
/// (ApartmentCreated/ApartmentOwnerChanged already carry the building's name/address
/// alongside the apartment's own fields), so there's never a reason to sync one
/// without the other.
/// </summary>
internal sealed class SyncApartmentCommandHandler : ICommandHandler<SyncApartmentCommand>
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncApartmentCommandHandler(
        IBuildingRepository buildingRepository,
        IApartmentRepository apartmentRepository,
        IUnitOfWork unitOfWork)
    {
        _buildingRepository = buildingRepository;
        _apartmentRepository = apartmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncApartmentCommand request, CancellationToken cancellationToken)
    {
        var building = await _buildingRepository.GetByIdDefaultAsync(request.BuildingId, cancellationToken);
        if (building is not null)
        {
            building.Update(request.BuildingName, request.BuildingAddress);
        }
        else
        {
            building = Building.Create(request.BuildingId, request.BuildingName, request.BuildingAddress);
            _buildingRepository.Add(building);
        }

        var apartment = await _apartmentRepository.GetByIdDefaultAsync(request.ApartmentId, cancellationToken);
        if (apartment is not null)
        {
            apartment.UpdateDetails(request.ApartmentNumber, request.AreaSquareMeters);
        }
        else
        {
            apartment = Apartment.Create(request.ApartmentId, request.BuildingId, request.ApartmentNumber, request.AreaSquareMeters);
            _apartmentRepository.Add(apartment);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
