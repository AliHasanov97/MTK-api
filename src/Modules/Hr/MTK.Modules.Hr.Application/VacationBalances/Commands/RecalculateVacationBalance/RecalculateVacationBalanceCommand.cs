using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Application.VacationBalances.Queries.GetVacationBalanceByEmployeeId;

namespace MTK.Modules.Hr.Application.VacationBalances.Commands.RecalculateVacationBalance;

/// <summary>
/// CARİ iş ilinin əlavə günlərini (staj/Qarabağ/uşaq/əlil-uşaq bonusları) işçinin canlı məlumatına
/// görə əl ilə yenidən hesablayır. Avtomatik olaraq EmployeeWorkHistoryChangedDomainEvent-də də
/// baş verir (bax: EmployeeWorkHistoryChangedDomainEventHandler), bu, həmin hesablamanın outbox-un
/// gecikməsini gözləmədən, HR-ın özü istədiyi an işə saldığı versiyasıdır.
/// </summary>
public sealed record RecalculateVacationBalanceCommand(Guid EmployeeId) : ICommand<VacationBalanceResponse>;
