namespace MTK.Modules.Hr.Application.FileAttachments;

/// <summary>Object storage-də HR faylının açarı (FileAttachment.Id əsasında).</summary>
internal static class FileAttachmentStorageKey
{
    public static string For(Guid attachmentId) => $"hr/{attachmentId}";
}
