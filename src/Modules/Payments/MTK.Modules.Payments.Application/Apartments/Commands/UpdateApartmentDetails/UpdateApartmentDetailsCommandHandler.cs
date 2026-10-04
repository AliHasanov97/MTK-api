using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Apartments.Commands.UpdateApartmentDetails;

internal sealed class UpdateApartmentDetailsCommandHandler : ICommandHandler<UpdateApartmentDetailsCommand>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateApartmentDetailsCommandHandler(IApartmentRepository apartmentRepository, IUnitOfWork unitOfWork)
    {
        _apartmentRepository = apartmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateApartmentDetailsCommand request, CancellationToken cancellationToken)
    {
        var apartment = await _apartmentRepository.GetByIdDefaultAsync(request.ApartmentId, cancellationToken);
        if (apartment is null)
        {
            return Result.Success();
        }

        apartment.UpdateDetails(request.ApartmentNumber ?? apartment.ApartmentNumber, request.AreaSquareMeters);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
