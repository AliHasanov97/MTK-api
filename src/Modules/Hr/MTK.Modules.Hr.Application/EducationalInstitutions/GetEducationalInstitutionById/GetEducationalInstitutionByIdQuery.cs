using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.GetEducationalInstitutionById;

public sealed record GetEducationalInstitutionByIdQuery(Guid Id) : IQuery<GetEducationalInstitutionByIdResponse>;
