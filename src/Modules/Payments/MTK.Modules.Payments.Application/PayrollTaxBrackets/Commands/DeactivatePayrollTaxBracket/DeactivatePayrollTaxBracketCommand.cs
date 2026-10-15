using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.PayrollTaxBrackets.Commands.DeactivatePayrollTaxBracket;

/// <summary>Bir dərəcə pilləsini (verilmiş tarixdən) bağlayır — əvəzedici yaratmadan, məs. səhvi düzəltmək üçün.</summary>
public sealed record DeactivatePayrollTaxBracketCommand(Guid Id, DateTimeOffset EffectiveTo) : ICommand;
