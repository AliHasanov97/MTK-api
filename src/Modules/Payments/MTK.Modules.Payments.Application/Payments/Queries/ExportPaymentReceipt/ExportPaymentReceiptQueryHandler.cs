using MTK.Common.Application.Authorization;
using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Domain.Apartments;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Garages;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.ExportPaymentReceipt;

internal sealed class ExportPaymentReceiptQueryHandler : IQueryHandler<ExportPaymentReceiptQuery, ExportFileResult>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IPropertyOwnershipRepository _propertyOwnershipRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IGarageRepository _garageRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPaymentReceiptExportService _exportService;

    public ExportPaymentReceiptQueryHandler(
        IPaymentRepository paymentRepository,
        IPaymentAllocationRepository paymentAllocationRepository,
        IChargeRepository chargeRepository,
        IPropertyOwnershipRepository propertyOwnershipRepository,
        IOwnerRepository ownerRepository,
        IVendorRepository vendorRepository,
        IApartmentRepository apartmentRepository,
        IGarageRepository garageRepository,
        IBuildingRepository buildingRepository,
        IAuditLogRepository auditLogRepository,
        IUserRepository userRepository,
        IPaymentReceiptExportService exportService)
    {
        _paymentRepository = paymentRepository;
        _paymentAllocationRepository = paymentAllocationRepository;
        _chargeRepository = chargeRepository;
        _propertyOwnershipRepository = propertyOwnershipRepository;
        _ownerRepository = ownerRepository;
        _vendorRepository = vendorRepository;
        _apartmentRepository = apartmentRepository;
        _garageRepository = garageRepository;
        _buildingRepository = buildingRepository;
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
        _exportService = exportService;
    }

    private static readonly Dictionary<string, string> RoleTitles = new()
    {
        [Roles.BuildingManager] = "Komendantı",
        [Roles.Accountant] = "Xəzinədarı",
        [Roles.Admin] = "Rəhbərliyi",
    };

    public async Task<Result<ExportFileResult>> Handle(
        ExportPaymentReceiptQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdDefaultAsync(request.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure<ExportFileResult>(new Error(
                "Payment.NotFound",
                $"Ödəniş tapılmadı: {request.PaymentId}"));
        }

        var allocations = await _paymentAllocationRepository.GetByPaymentIdAsync(payment.Id, cancellationToken);
        var chargeIds = allocations.Select(a => a.ChargeId).Distinct().ToList();
        var charges = await _chargeRepository.ListFromIdsAsync(chargeIds, cancellationToken);
        var chargesById = charges.ToDictionary(c => c.Id);

        // A FIFO/general payment can settle charges across SEVERAL different units at
        // once (e.g. part of it clears an apartment's debt, the rest a garage's) — every
        // line needs its own property label, never just the payment's single resolved
        // unit, or a split payment silently reads as if the whole amount went to one unit.
        var apartmentIds = charges.Where(c => c.ApartmentId.HasValue).Select(c => c.ApartmentId!.Value).Distinct().ToList();
        var garageIds = charges.Where(c => c.GarageId.HasValue).Select(c => c.GarageId!.Value).Distinct().ToList();

        var apartmentsById = apartmentIds.Count > 0
            ? (await _apartmentRepository.ListFromIdsWithBuildingAsync(apartmentIds, cancellationToken)).ToDictionary(a => a.Id)
            : new Dictionary<Guid, Apartment>();
        var garagesById = garageIds.Count > 0
            ? (await _garageRepository.ListFromIdsAsync(garageIds, cancellationToken)).ToDictionary(g => g.Id)
            : new Dictionary<Guid, Garage>();

        var lines = allocations
            .Select(a =>
            {
                chargesById.TryGetValue(a.ChargeId, out var charge);
                // Monthly auto-generated charges carry no free-text Description (only
                // manual/vendor charges do) — without a period fallback here, every line
                // of a bulk payment covering several months would show as plain "Haqq"
                // with no way to tell which month each amount belongs to.
                string description = charge?.Description ?? "Kommunal haqqı";

                string? propertyLabel = charge?.ApartmentId is { } aptId && apartmentsById.TryGetValue(aptId, out var apt)
                    ? $"{apt.ApartmentNumber} saylı mənzil"
                    : charge?.GarageId is { } garId && garagesById.TryGetValue(garId, out var gar)
                        ? $"{gar.GarageNumber} saylı qaraj"
                        : null;

                return new PaymentReceiptLine(description, a.Amount, charge?.Period, propertyLabel);
            })
            .ToList();

        // Which unit this payment is for: the payment's own PropertyId when it was
        // targeted at one (see CreatePaymentCommand), otherwise whichever property the
        // charges it settled mostly belong to. Only meaningful as a single answer when
        // every charge belongs to the SAME unit — when they don't, PaymentReceiptLine's
        // own PropertyLabel (set above) is what the PDF itemises by instead.
        Guid? propertyId = payment.PropertyId ?? charges
            .Where(c => (c.ApartmentId ?? c.GarageId).HasValue)
            .GroupBy(c => (c.ApartmentId ?? c.GarageId)!.Value)
            .OrderByDescending(g => g.Count())
            .Select(g => (Guid?)g.Key)
            .FirstOrDefault();

        string? propertyNumber = null;
        PropertyType? propertyType = payment.PropertyType;
        PropertyOwnership? ownership = null;
        if (propertyId.HasValue)
        {
            ownership = await _propertyOwnershipRepository.GetByPropertyIdAsync(propertyId.Value, cancellationToken);

            // Real unit number comes straight from the Apartment/Garage shadow navigation.
            propertyNumber = ownership?.Apartment?.ApartmentNumber ?? ownership?.Garage?.GarageNumber;
            propertyType ??= ownership?.ApartmentId.HasValue == true ? PropertyType.Apartment
                : ownership?.GarageId.HasValue == true ? PropertyType.Garage
                : null;
        }

        // The receipt's "for [month] [year]" line only makes sense when every charge
        // this payment settled shares one billing period — leave it null otherwise
        // (several months, or a pure advance with no charge behind it) and let the PDF
        // fall back to the payment's own date.
        var distinctPeriods = charges
            .Select(c => c.Period)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct()
            .ToList();
        string? period = distinctPeriods.Count == 1 ? distinctPeriods[0] : null;

        string? payerName = payment.PartyType == PartyType.Owner
            ? (await _ownerRepository.GetByIdDefaultAsync(payment.PartyId, cancellationToken))?.FullName
            : (await _vendorRepository.GetByIdDefaultAsync(payment.PartyId, cancellationToken))?.Name;

        // Apartment payments take the real unit's building; garages (and any payment
        // with no resolvable unit) fall back to the complex's one building, since
        // Garage carries no BuildingId at all in Buildings' own domain. For a
        // multi-property split, any apartment among the settled charges gives the same
        // answer (single-complex system — one Building either way).
        string? buildingAddress = ownership?.Apartment?.Building?.Address
            ?? apartmentsById.Values.FirstOrDefault()?.Building?.Address
            ?? (await _buildingRepository.GetFirstAsync(cancellationToken))?.Address;

        var creationAudit = await _auditLogRepository.GetForEntityActionAsync(
            "Payment",
            payment.Id,
            "Created",
            cancellationToken);

        string? issuerName = null;
        string? issuerTitle = null;
        if (creationAudit?.UserId is { } issuerUserId)
        {
            issuerName = (await _userRepository.GetByIdDefaultAsync(issuerUserId, cancellationToken))?.FullName;
        }
        if (creationAudit?.ActorRole is { } actorRole)
        {
            RoleTitles.TryGetValue(actorRole, out issuerTitle);
        }

        var data = new PaymentReceiptData(
            payment.Id,
            payment.Amount,
            payment.PaymentMethod,
            payment.PaymentDate,
            payment.Notes,
            payment.PartyType,
            payerName,
            propertyNumber,
            propertyType,
            period,
            lines,
            issuerName,
            issuerTitle,
            buildingAddress);

        var stream = _exportService.ExportToPdf(data);
        var fileName = $"Qebz_{payment.PaymentDate:yyyy-MM-dd}_{payment.Id.ToString()[..8]}.pdf";

        return new ExportFileResult(stream.ToArray(), fileName, "application/pdf");
    }
}
