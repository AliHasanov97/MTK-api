namespace MTK.Common.Presentation.Responses;

/// <summary>
/// Ümumi Id və Name olan obyekt (fivestar-api-dəki kimi)
/// </summary>
public class ResponseObjectWithName
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public static ResponseObjectWithName Create(Guid id, string name)
    {
        return new ResponseObjectWithName
        {
            Id = id,
            Name = name
        };
    }
}
