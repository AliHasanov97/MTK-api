using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentAllocations;

internal sealed class GetPaymentAllocationsQueryHandler
    : IQueryHandler<GetPaymentAllocationsQuery, IReadOnlyCollection<PaymentAllocationDetailResponse>>
{
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IGarageRepository _garageRepository;

    public GetPaymentAllocationsQueryHandler(
        IPaymentAllocationRepository paymentAllocationRepository,
        IChargeRepository chargeRepository,
        IApartmentRepository apartmentRepository,
        IGarageRepository garageRepository)
    {
        _paymentAllocationRepository = paymentAllocationRepository;
        _chargeRepository = chargeRepository;
        _apartmentRepository = apartmentRepository;
        _garageRepository = garageRepository;
    }

    public async Task<Result<IReadOnlyCollection<PaymentAllocationDetailResponse>>> Handle(
        GetPaymentAllocationsQuery request,
        CancellationToken cancellationToken)
    {
        var allocations = await _paymentAllocationRepository.GetByPaymentIdAsync(request.PaymentId, cancellationToken);

        if (allocations.Count == 0)
        {
            return Result.Success<IReadOnlyCollection<PaymentAllocationDetailResponse>>(
                Array.Empty<PaymentAllocationDetailResponse>());
        }

        var chargeIds = allocations.Select(a => a.ChargeId).Distinct().ToList();
        var charges = await _chargeRepository.ListFromIdsAsync(chargeIds, cancellationToken);
        var chargesById = charges.ToDictionary(c => c.Id);

        var apartmentIds = charges.Where(c => c.ApartmentId.HasValue).Select(c => c.ApartmentId!.Value).Distinct().ToList();
        var garageIds = charges.Where(c => c.GarageId.HasValue).Select(c => c.GarageId!.Value).Distinct().ToList();

        var apartmentLabels = apartmentIds.Count > 0
            ? (await _apartmentRepository.ListFromIdsWithBuildingAsync(apartmentIds, cancellationToken))
                .ToDictionary(a => a.Id, a => $"Mənzil {a.ApartmentNumber} — {a.Building.Name}")
            : new Dictionary<Guid, string>();
        var garageLabels = garageIds.Count > 0
            ? (await _garageRepository.ListFromIdsAsync(garageIds, cancellationToken))
                .ToDictionary(g => g.Id, g => $"Qaraj {g.GarageNumber}")
            : new Dictionary<Guid, string>();

        var response = allocations
            .Where(a => chargesById.ContainsKey(a.ChargeId))
            .Select(a =>
            {
                var charge = chargesById[a.ChargeId];
                string? propertyLabel = charge.ApartmentId is { } apartmentId
                    ? apartmentLabels.GetValueOrDefault(apartmentId)
                    : charge.GarageId is { } garageId
                        ? garageLabels.GetValueOrDefault(garageId)
                        : null;

                return new PaymentAllocationDetailResponse(
                    a.Id,
                    charge.Id,
                    charge.ApartmentId,
                    charge.GarageId,
                    charge.Period,
                    charge.Description,
                    charge.Amount,
                    a.Amount,
                    a.RemainingDebtAfterPayment,
                    propertyLabel);
            })
            .ToList();

        return response;
    }
}
