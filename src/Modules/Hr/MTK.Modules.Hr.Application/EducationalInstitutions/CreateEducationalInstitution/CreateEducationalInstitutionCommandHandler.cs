using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.EducationalInstitutions;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.CreateEducationalInstitution;

internal sealed class CreateEducationalInstitutionCommandHandler : ICommandHandler<CreateEducationalInstitutionCommand, CreateEducationalInstitutionResponse>
{
    private readonly IEducationalInstitutionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEducationalInstitutionCommandHandler(
        IEducationalInstitutionRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CreateEducationalInstitutionResponse>> Handle(
        CreateEducationalInstitutionCommand request,
        CancellationToken cancellationToken)
    {
        var institution = EducationalInstitution.Create(Guid.NewGuid(), request.Name, request.Type);
        await _repository.AddAsync(institution, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var saved = await _repository.GetByIdDefaultAsync(institution.Id, cancellationToken);
        return Result.Success(_mapper.Map<CreateEducationalInstitutionResponse>(saved));
    }
}
