using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Appeals;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Appeals.Queries.GetAppealById;

internal sealed class GetAppealByIdQueryHandler : IQueryHandler<GetAppealByIdQuery, AppealDetailResponse>
{
    private readonly IAppealRepository _appealRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IGarageRepository _garageRepository;

    public GetAppealByIdQueryHandler(
        IAppealRepository appealRepository,
        IUserRepository userRepository,
        IApartmentRepository apartmentRepository,
        IGarageRepository garageRepository)
    {
        _appealRepository = appealRepository;
        _userRepository = userRepository;
        _apartmentRepository = apartmentRepository;
        _garageRepository = garageRepository;
    }

    public async Task<Result<AppealDetailResponse>> Handle(
        GetAppealByIdQuery request,
        CancellationToken cancellationToken)
    {
        var appeal = await _appealRepository.GetWithCommentsAsync(request.AppealId, cancellationToken);

        if (appeal is null)
        {
            return Result.Failure<AppealDetailResponse>(
                new Error("Appeal.NotFound", "Müraciət tapılmadı"));
        }

        // Bütün user ID-lərini toplayaq (submitter, comment users)
        var userIds = new List<Guid> { appeal.SubmitterId };
        userIds.AddRange(appeal.Comments.Select(c => c.UserId));

        var distinctUserIds = userIds.Distinct().ToList();
        var userNames = await _userRepository.GetUserNamesByIdsAsync(distinctUserIds, cancellationToken);

        // Apartment və garage label-larını alaq
        string? apartmentLabel = null;
        if (appeal.ApartmentId.HasValue)
        {
            var apartment = await _apartmentRepository.GetByIdAsync(appeal.ApartmentId.Value, cancellationToken);
            apartmentLabel = apartment?.ApartmentNumber;
        }

        string? garageLabel = null;
        if (appeal.GarageId.HasValue)
        {
            var garage = await _garageRepository.GetByIdAsync(appeal.GarageId.Value, cancellationToken);
            garageLabel = garage?.GarageNumber;
        }

        var comments = appeal.Comments
            .Select(c => new AppealCommentResponse(
                c.Id,
                c.UserId,
                userNames.TryGetValue(c.UserId, out var commentUserName) ? commentUserName : "Naməlum istifadəçi",
                c.Text,
                c.CreatedAt))
            .OrderBy(c => c.CreatedAt)
            .ToList();

        var response = new AppealDetailResponse(
            appeal.Id,
            appeal.Title,
            appeal.Description,
            appeal.Category,
            appeal.Status,
            appeal.Priority,
            appeal.SubmitterId,
            userNames.TryGetValue(appeal.SubmitterId, out var submitterName) ? submitterName : "Naməlum istifadəçi",
            appeal.ApartmentId,
            apartmentLabel,
            appeal.GarageId,
            garageLabel,
            appeal.CreatedAt,
            appeal.UpdatedAt,
            appeal.ResolvedAt,
            appeal.ClosedAt,
            comments);

        return response;
    }
}
