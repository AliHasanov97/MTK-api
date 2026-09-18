namespace MTK.Common.Domain.ValueObjects;

public record Currency
{
    internal static readonly Currency Usd = new("USD");
    public static readonly Currency Azn = new("AZN");
    public static readonly Currency None = new("");

    private Currency(string code) => Code = code;

    public string Code { get; init; }

    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(x => x.Code == code) ??
               throw new ArgumentException($"Unknown currency code: {code}");
    }

    public static readonly IReadOnlyCollection<Currency> All = new[]
    {
        Usd,
        Azn
    };
}
