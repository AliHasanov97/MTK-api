using AutoMapper;
using MTK.Modules.Buildings.Application.Buildings.Queries.GetBuildingById;
using MTK.Modules.Buildings.Application.Owners.Queries.SearchOwners;
using MTK.Modules.Buildings.Application.OwnershipHistories.Queries.GetOwnershipHistoryByApartment;
using MTK.Modules.Buildings.Domain.Buildings;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.OwnershipHistories;

namespace MTK.Modules.Buildings.Application;

/// <summary>
/// Covers only the "pure 1:1" query-response mappings. Handlers that join a related
/// entity (Garage/Apartment + Owner, Apartment + Building) or resolve an audit-log
/// UserId to a display name via a second repository call stay hand-written.
/// </summary>
public sealed class BuildingsMappingProfile : Profile
{
    public BuildingsMappingProfile()
    {
        // FullAddress has no matching property on the entity — it's a method call
        // on the Address value object. Status (enum -> string) maps automatically.
        // BuildingResponse is an immutable record (positional ctor, no settable
        // properties), so this must be a constructor-parameter mapping (ForCtorParam)
        // — plain ForMember makes AutoMapper fall back to "construct via parameterless
        // ctor + set properties", which records without `init` setters don't support.
        CreateMap<Building, BuildingResponse>()
            .ForCtorParam(nameof(BuildingResponse.FullAddress), o => o.MapFrom(s => s.Address.GetFullAddress()));

        CreateMap<Owner, OwnerListItem>();

        CreateMap<OwnershipHistory, OwnershipHistoryResponse>();
    }
}
