namespace MTK.Modules.Buildings.Domain.ValueObjects;

/// <summary>
/// Ünvan value object
/// </summary>
public sealed record Address
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string? District { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }

    private Address() { }

    public Address(
        string street,
        string city,
        string? district = null,
        string? postalCode = null,
        string? country = "Azerbaijan")
    {
        Street = street;
        City = city;
        District = district;
        PostalCode = postalCode;
        Country = country;
    }

    public string GetFullAddress()
    {
        var parts = new List<string?> { Street, District, City, Country }
            .Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(", ", parts);
    }
}
