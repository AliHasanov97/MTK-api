using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.EducationalInstitutions;
using System.Text.Json.Serialization;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.UpdateEducationalInstitution;

public sealed class UpdateEducationalInstitutionCommand : ICommand<UpdateEducationalInstitutionResponse>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EducationalInstitutionType Type { get; set; }
}
