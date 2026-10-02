using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Owners.Queries.GetOwnerById;

namespace MTK.Modules.Buildings.Application.Owners.Queries.GetOwnerByUserId;

/// <summary>
/// "Mənim profilim" — sakinin öz Owner qeydini tapır (öz UserId-si ilə).
/// Admin/Komandant başqasının profilinə <see cref="GetOwnerById.GetOwnerByIdQuery"/>
/// ilə baxır; bu sorğu sakinin öz-özünə baxması üçündür.
/// </summary>
public sealed record GetOwnerByUserIdQuery(Guid UserId) : IQuery<OwnerResponse>;
