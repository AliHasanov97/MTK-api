using AutoMapper;
using MTK.Modules.Payments.Application.Apartments;
using MTK.Modules.Payments.Application.Buildings;
using MTK.Modules.Payments.Application.Charges.Queries.GetChargesByOwner;
using MTK.Modules.Payments.Application.Garages;
using MTK.Modules.Payments.Application.Charges.Queries.SearchCharges;
using MTK.Modules.Payments.Application.CompanyBalances.Queries.GetCompanyBalance;
using MTK.Modules.Payments.Application.Nomenclatures.Queries.SearchNomenclatureShadows;
using MTK.Modules.Payments.Application.OwnerBalances.Queries.GetOwnerBalance;
using MTK.Modules.Payments.Application.Owners;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;
using MTK.Modules.Payments.Application.Payments.Queries.SearchPayments;
using MTK.Modules.Payments.Application.PropertyOwnerships;
using MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;
using MTK.Modules.Payments.Application.Transactions.Queries.SearchTransactions;
using MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;
using MTK.Modules.Payments.Application.Vendors;
using GetVendorByIdVendorResponse = MTK.Modules.Payments.Application.Vendors.Queries.GetVendorById.VendorResponse;
using MTK.Modules.Payments.Application.Vendors.Queries.SearchVendors;
using MTK.Modules.Payments.Domain.Apartments;
using MTK.Modules.Payments.Domain.Buildings;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.CompanyBalances;
using MTK.Modules.Payments.Domain.Garages;
using MTK.Modules.Payments.Domain.Nomenclatures;
using MTK.Modules.Payments.Domain.OwnerBalances;
using MTK.Modules.Payments.Domain.Owners;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Transactions;
using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Application;

/// <summary>
/// Covers only the "pure 1:1" query-response mappings — a single entity's own
/// properties copied straight across, with at most a rename. Handlers that join in
/// a second repository call (vendor name, allocation totals, audit-log user lookup)
/// or compute an aggregate across a list stay hand-written; AutoMapper would need
/// custom resolvers there and wouldn't actually remove the complexity.
/// </summary>
public sealed class PaymentsMappingProfile : Profile
{
    public PaymentsMappingProfile()
    {
        CreateMap<Rate, RateResponse>();
        CreateMap<Owner, OwnerResponse>();
        CreateMap<Building, BuildingResponse>();
        CreateMap<Apartment, ApartmentResponse>();
        CreateMap<Garage, GarageResponse>();
        CreateMap<Vendor, VendorResponse>();
        CreateMap<OwnerBalance, OwnerBalanceResponse>();
        CreateMap<CompanyBalance, CompanyBalanceResponse>();
        CreateMap<Transaction, TransactionSearchResult>();

        CreateMap<Vendor, GetVendorByIdVendorResponse>();
        CreateMap<Vendor, VendorSearchResult>();

        CreateMap<NomenclatureShadow, NomenclatureShadowSearchResult>();

        // Charge/Payment now carry real OwnerId/VendorId/ApartmentId/GarageId columns
        // (no more PartyId/PropertyId discriminator-pair to alias), so these map
        // straight across by name — only ChargeDate still needs an explicit rename.
        CreateMap<Charge, ChargeResponse>();
        CreateMap<Charge, ChargeSearchResult>();
        CreateMap<Charge, VendorChargeResponse>()
            .ForCtorParam(nameof(VendorChargeResponse.ChargeDate), o => o.MapFrom(s => s.IssuedOn));

        CreateMap<Payment, PaymentResponse>();
        CreateMap<Payment, PaymentSearchResult>();

        // PropertyOwnership cache mappings - flatten navigation properties for Redis storage
        CreateMap<Apartment, ApartmentData>();
        CreateMap<Garage, GarageData>();
        CreateMap<PropertyOwnership, PropertyOwnershipResponse>();
    }
}
