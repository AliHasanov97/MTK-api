using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.Jobs;

public class Job : SearchableEntity
{
    private Job() { }

    public string Name { get; private set; } = string.Empty;

    public static Job Create(Guid id, string name)
    {
        return new Job
        {
            Id = id,
            Name = name
        };
    }

    public void Update(string name)
    {
        Name = name;
    }
}
