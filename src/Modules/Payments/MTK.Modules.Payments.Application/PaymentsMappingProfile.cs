using AutoMapper;
using MTK.Modules.Payments.Application.Charges.Queries.GetChargesByOwner;
using MTK.Modules.Payments.Application.Charges.Queries.SearchCharges;
using MTK.Modules.Payments.Application.CompanyBalances.Queries.GetCompanyBalance;
using MTK.Modules.Payments.Application.OwnerBalances.Queries.GetOwnerBalance;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;
using MTK.Modules.Payments.Application.Payments.Queries.SearchPayments;
using MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;
using MTK.Modules.Payments.Application.Transactions.Queries.SearchTransactions;
using MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;
using MTK.Modules.Payments.Application.Vendors.Queries.GetVendorById;
using MTK.Modules.Payments.Application.Vendors.Queries.SearchVendors;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.CompanyBalances;
using MTK.Modules.Payments.Domain.OwnerBalances;
using MTK.Modules.Payments.Domain.Payments;
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
        CreateMap<OwnerBalance, OwnerBalanceResponse>();
        CreateMap<CompanyBalance, CompanyBalanceResponse>();
        CreateMap<Transaction, TransactionSearchResult>();

        CreateMap<Vendor, VendorResponse>();
        CreateMap<Vendor, VendorSearchResult>();

        // Charge.PartyId is the owner/vendor discriminator column — each response
        // names it for the party it actually represents. These responses are
        // immutable records (positional constructor, no settable properties), so a
        // renamed member must be wired as a constructor-parameter mapping
        // (ForCtorParam) — ForMember alone makes AutoMapper fall back to "construct
        // via parameterless ctor + set properties", which these types don't have.
        CreateMap<Charge, ChargeResponse>()
            .ForCtorParam(nameof(ChargeResponse.OwnerId), o => o.MapFrom(s => s.PartyId));
        CreateMap<Charge, ChargeSearchResult>()
            .ForCtorParam(nameof(ChargeSearchResult.OwnerId), o => o.MapFrom(s => s.PartyId));
        CreateMap<Charge, VendorChargeResponse>()
            .ForCtorParam(nameof(VendorChargeResponse.VendorId), o => o.MapFrom(s => s.PartyId))
            .ForCtorParam(nameof(VendorChargeResponse.ChargeDate), o => o.MapFrom(s => s.IssuedOn));

        CreateMap<Payment, PaymentResponse>()
            .ForCtorParam(nameof(PaymentResponse.OwnerId), o => o.MapFrom(s => s.PartyId));
        CreateMap<Payment, PaymentSearchResult>()
            .ForCtorParam(nameof(PaymentSearchResult.OwnerId), o => o.MapFrom(s => s.PartyId));
    }
}
