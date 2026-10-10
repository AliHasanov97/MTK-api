using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.Modules.Hr.Application.Jobs.AddJob;

internal sealed class AddJobCommandHandler : ICommandHandler<AddJobCommand, AddJobResponse>
{
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddJobCommandHandler(IJobRepository jobRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AddJobResponse>> Handle(AddJobCommand request, CancellationToken cancellationToken)
    {
        var job = Job.Create(Guid.NewGuid(), request.Name);
        await _jobRepository.AddAsync(job, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var saved = await _jobRepository.GetByIdDefaultAsync(job.Id, cancellationToken);
        return Result.Success(_mapper.Map<AddJobResponse>(saved));
    }
}
