using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EducationalInstitutions;

public class EducationalInstitution : SearchableEntity
{
    private EducationalInstitution() { }

    public string Name { get; private set; } = string.Empty;
    public EducationalInstitutionType Type { get; private set; }

    public static EducationalInstitution Create(Guid id, string name, EducationalInstitutionType type)
    {
        return new EducationalInstitution
        {
            Id = id,
            Name = name,
            Type = type
        };
    }

    public void Update(string name, EducationalInstitutionType type)
    {
        Name = name;
        Type = type;
    }
}
