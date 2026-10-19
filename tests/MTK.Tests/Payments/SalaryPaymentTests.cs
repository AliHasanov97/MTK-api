using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.SalaryPayments;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class SalaryPaymentTests
{
    [Fact]
    public void AddLine_RecalculatesBonusAdjustmentAndGrossTotals()
    {
        var payment = SalaryPayment.Create(Guid.NewGuid(), "  Aysel Aliyeva  ", 5, 2026, 1800m,
            new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero), PaymentMethod.BankTransfer);

        payment.AddLine(SalaryPaymentLineType.Bonus, "Monthly bonus", 200m);
        payment.AddLine(SalaryPaymentLineType.Adjustment, "Unpaid leave deduction", -75m);

        Assert.Equal("Aysel Aliyeva", payment.EmployeeFullName);
        Assert.Equal(200m, payment.TotalBonuses);
        Assert.Equal(-75m, payment.TotalAdjustments);
        Assert.Equal(1925m, payment.GrossSalary);
    }

    [Fact]
    public void MarkAsPaid_RequiresAtLeastOneLineAndPreventsFurtherEdits()
    {
        var payment = SalaryPayment.Create(Guid.NewGuid(), "Aysel Aliyeva", 5, 2026, 1800m,
            DateTimeOffset.UtcNow, PaymentMethod.Cash);

        Assert.Throws<InvalidOperationException>(() => payment.MarkAsPaid());
        payment.AddLine(SalaryPaymentLineType.Bonus, "Bonus", 100m);
        payment.MarkAsPaid();

        Assert.Equal(SalaryPaymentStatus.Paid, payment.Status);
        Assert.Throws<InvalidOperationException>(() => payment.AddLine(SalaryPaymentLineType.Bonus, "Late bonus", 20m));
        Assert.Throws<InvalidOperationException>(() => payment.MarkAsPaid());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Create_RejectsInvalidSalaryMonth(int month)
    {
        Assert.Throws<ArgumentException>(() => SalaryPayment.Create(Guid.NewGuid(), "Employee", month, 2026, 100m,
            DateTimeOffset.UtcNow, PaymentMethod.Cash));
    }
}
