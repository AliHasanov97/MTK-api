using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.VacationPays;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class VacationPayTests
{
    [Fact]
    public void MarkAsPaid_UpdatesPaymentDetailsAndRaisesPaidEvent()
    {
        var pay = VacationPay.Create(Guid.NewGuid(), Guid.NewGuid(), " Aysel Aliyeva ", 1300m, 42.76m, 14,
            598.64m, new DateOnly(2026, 7, 10), new DateOnly(2026, 7, 23), new DateOnly(2026, 7, 7));
        var paidAt = new DateTimeOffset(2026, 7, 5, 10, 0, 0, TimeSpan.Zero);

        pay.MarkAsPaid(PaymentMethod.BankTransfer, paidAt);

        Assert.Equal("Aysel Aliyeva", pay.EmployeeFullName);
        Assert.Equal(VacationPayStatus.Paid, pay.Status);
        Assert.Equal(PaymentMethod.BankTransfer, pay.PaymentMethod);
        Assert.Equal(paidAt, pay.PaidAt);
        Assert.Equal(2, pay.DomainEvents.Count);
        Assert.Contains(pay.DomainEvents, domainEvent => domainEvent.GetType().Name == "VacationPayPaidDomainEvent");
        Assert.Throws<InvalidOperationException>(() => pay.MarkAsPaid(PaymentMethod.Cash, paidAt.AddDays(1)));
    }

    [Fact]
    public void Create_RejectsVacationPayWithoutPositiveDays()
    {
        Assert.Throws<ArgumentException>(() => VacationPay.Create(Guid.NewGuid(), Guid.NewGuid(), "Employee", 1000m,
            32.89m, 0, 0m, new DateOnly(2026, 7, 10), new DateOnly(2026, 7, 10), new DateOnly(2026, 7, 7)));
    }
}
