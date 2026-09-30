using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.OwnerBalances.Queries.GetOwnerBalance;

namespace MTK.Modules.Payments.Application.OwnerBalances.Commands.RecalculateOwnerBalance;

/// <summary>
/// Sahib balansı proyeksiyasını aqreqatdan (borclar + tamamlanmış ödənişlər) yenidən
/// hesablayır. Balans yazıları artıq mütləq dəyər olduğu üçün normal axında sürüşmə
/// olmur; bu əmr keçmişdə yaranmış fərqi düzəltmək və yoxlamaq üçündür.
/// </summary>
public sealed record RecalculateOwnerBalanceCommand(Guid OwnerId) : ICommand<OwnerBalanceResponse>;
