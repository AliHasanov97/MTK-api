using Microsoft.AspNetCore.Http;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.FileAttachments.Commands.UploadFileAttachment;

public sealed record UploadFileAttachmentCommand(
    IFormFile File,
    Guid? BuildingId,
    Guid? ApartmentId,
    Guid? GarageId,
    Guid? OwnerId) : ICommand<Guid>;
