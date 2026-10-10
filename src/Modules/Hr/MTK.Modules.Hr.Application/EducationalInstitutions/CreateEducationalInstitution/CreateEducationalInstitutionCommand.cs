using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.EducationalInstitutions;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.CreateEducationalInstitution;

public sealed class CreateEducationalInstitutionCommand : ICommand<CreateEducationalInstitutionResponse>
{
    public string Name { get; set; } = string.Empty;
    public EducationalInstitutionType Type { get; set; }
}
