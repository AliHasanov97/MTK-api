namespace MTK.Common.Infrastructure.Options;

public sealed class CloudflareR2Options
{
    /// <summary>Cloudflare hesab ID-si — S3 endpoint-i buradan qurulur: https://{AccountId}.r2.cloudflarestorage.com</summary>
    public string AccountId { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string BucketName { get; init; } = string.Empty;
}
