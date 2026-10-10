using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.EducationalInstitutions;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.UpdateEducationalInstitution;

internal sealed class UpdateEducationalInstitutionCommandHandler : ICommandHandler<UpdateEducationalInstitutionCommand, UpdateEducationalInstitutionResponse>
{
    private readonly IEducationalInstitutionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateEducationalInstitutionCommandHandler(
        IEducationalInstitutionRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UpdateEducationalInstitutionResponse>> Handle(
        UpdateEducationalInstitutionCommand request,
        CancellationToken cancellationToken)
    {
        var institution = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (institution is null)
            return Result.Failure<UpdateEducationalInstitutionResponse>(EducationalInstitutionErrors.NotFound);

        institution.Update(request.Name, request.Type);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<UpdateEducationalInstitutionResponse>(institution));
    }
}
