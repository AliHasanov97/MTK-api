using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollAdjustment;

/// <summary>
/// HR-dəki bir əmrdən (məzuniyyət/ödənişsiz məzuniyyət/kompensasiya və s.) gələn iş saatı əsaslı
/// əmək haqqı düzəlişini bu modulda saxlayır. <paramref name="OrderId"/> eyni zamanda
/// idempotentlik açarıdır (HR əmrinin Id-si). Pul = baza maaş × (Hours / NormHours).
/// </summary>
public sealed record SyncPayrollAdjustmentCommand(
    Guid OrderId,
    Guid EmployeeId,
    PayrollAdjustmentReason Reason,
    decimal Hours,
    decimal NormHours,
    int SalaryMonth,
    int SalaryYear) : ICommand;
