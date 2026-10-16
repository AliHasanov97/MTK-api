using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.SalaryPayments;

/// <summary>
/// Maaş ödənişinin bir komponenti (əsas maaş, bonus, düzəliş).
/// SalaryPayment aggregate-inin child entity-si.
/// </summary>
public sealed class SalaryPaymentLine : Entity
{
    private SalaryPaymentLine() : base() { }

    public Guid SalaryPaymentId { get; private set; }
    public SalaryPaymentLineType LineType { get; private set; }
    public string Description { get; private set; } = string.Empty;

    /// <summary>Məbləğ (işarəli - adjustment mənfi ola bilər).</summary>
    public decimal Amount { get; private set; }

    /// <summary>PayrollBonus.Id - link to source HR record (audit trail).</summary>
    public Guid? SourceBonusId { get; private set; }

    /// <summary>PayrollAdjustment.Id - link to source HR record (audit trail).</summary>
    public Guid? SourceAdjustmentId { get; private set; }

    internal static SalaryPaymentLine Create(
        Guid salaryPaymentId,
        SalaryPaymentLineType lineType,
        string description,
        decimal amount,
        Guid? sourceId = null)
    {
        var line = new SalaryPaymentLine
        {
            SalaryPaymentId = salaryPaymentId,
            LineType = lineType,
            Description = description,
            Amount = amount
        };

        // Link to source record based on line type
        if (sourceId.HasValue)
        {
            if (lineType == SalaryPaymentLineType.Bonus)
            {
                line.SourceBonusId = sourceId;
            }
            else if (lineType == SalaryPaymentLineType.Adjustment)
            {
                line.SourceAdjustmentId = sourceId;
            }
        }

        line.SetCreatedAt();
        return line;
    }
}
