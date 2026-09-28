using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.AssignOwnerToApartment;

internal sealed class AssignOwnerToApartmentCommandHandler
    : ICommandHandler<AssignOwnerToApartmentCommand>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignOwnerToApartmentCommandHandler(
        IApartmentRepository apartmentRepository,
        IOwnerRepository ownerRepository,
        IUnitOfWork unitOfWork)
    {
        _apartmentRepository = apartmentRepository;
        _ownerRepository = ownerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        AssignOwnerToApartmentCommand request,
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

        var owner = await _ownerRepository.GetByIdAsync(
            request.OwnerId,
            cancellationToken);

        if (owner is null)
        {
            return Result.Failure(new Error(
                "Owner.NotFound",
                $"Sahib tapılmadı: {request.OwnerId}"));
        }

        apartment.AssignOwner(request.OwnerId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Domain event handler will publish integration event

        return Result.Success();
    }
}
