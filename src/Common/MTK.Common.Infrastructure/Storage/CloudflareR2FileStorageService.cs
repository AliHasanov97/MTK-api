using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Storage;
using MTK.Common.Infrastructure.Options;

namespace MTK.Common.Infrastructure.Storage;

/// <summary>
/// R2 Cloudflare bucket-i S3-uyğun API üzərindən danışır (eyni Amazon S3 SDK, fərqli
/// ServiceURL) — ona görə burada Amazon.S3 istifadə olunur, ayrıca Cloudflare SDK-sı yoxdur.
/// </summary>
internal sealed class CloudflareR2FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly CloudflareR2Options _options;

    public CloudflareR2FileStorageService(IAmazonS3 s3Client, IOptions<CloudflareR2Options> options)
    {
        _s3Client = s3Client;
        _options = options.Value;
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string objectKey,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            InputStream = fileStream,
            ContentType = contentType,
            // Without this, the SDK signs a Stream body with its chunked
            // STREAMING-AWS4-HMAC-SHA256-PAYLOAD scheme, which R2 doesn't
            // implement ("STREAMING-AWS4-HMAC-SHA256-PAYLOAD not implemented").
            // Headers are still SigV4-signed — only the body itself goes
            // unsigned, which R2 (and S3) both accept.
            DisablePayloadSigning = true,
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        return objectKey;
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        await _s3Client.DeleteObjectAsync(
            new DeleteObjectRequest { BucketName = _options.BucketName, Key = objectKey },
            cancellationToken);
    }

    public async Task<Stream> DownloadAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        var response = await _s3Client.GetObjectAsync(
            new GetObjectRequest { BucketName = _options.BucketName, Key = objectKey },
            cancellationToken);

        return response.ResponseStream;
    }

    public string GetPresignedDownloadUrl(string objectKey, TimeSpan validFor)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            Expires = DateTime.UtcNow.Add(validFor),
        };

        return _s3Client.GetPreSignedURL(request);
    }
}
