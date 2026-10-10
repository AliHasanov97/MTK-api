using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.DeleteEducationalInstitution;

public sealed record DeleteEducationalInstitutionCommand(Guid Id) : ICommand;
