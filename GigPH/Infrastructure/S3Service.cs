using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace GigPH.Infrastructure;

public interface IS3Service
{
    Task<string> UploadAsync(Stream stream, string key, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string key, CancellationToken ct = default);
    Task DeleteAsync(string key, CancellationToken ct = default);
    string? GetPresignedUrl(string key, int expiryMinutes = 60);
}



public class S3Service : IS3Service 
{

    private readonly IOptions<S3Options> _options;
    private readonly IAmazonS3 _s3;
    public S3Service(IOptions<S3Options> options, IAmazonS3 s3)
    {
        _options = options;
        _s3 = s3;

    }

    public async Task<string> UploadAsync(Stream stream, string key, string contentType, CancellationToken ct = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.Value.BucketName,
            Key = key,
            InputStream =
                stream, // not really filepath cause we dont want to store our stuff in .Net but aws so shoudl go memory -> aws directly
            ContentType = contentType,
            AutoCloseStream =
                false, // reason why is aws auto destroys it but the thing is .NET if in memory creates an idisposable which auto deletes it as well 
        };
         await _s3.PutObjectAsync(request, ct);
         return key;
    }

    public async Task<Stream> DownloadAsync(string key, CancellationToken ct = default)
    {
        var response= await _s3.GetObjectAsync(_options.Value.BucketName, key, ct);
        return response.ResponseStream;
    }

    public async Task DeleteAsync(string key, CancellationToken ct = default)
    {
        await _s3.DeleteObjectAsync(_options.Value.BucketName, key, ct);
    }

    public string? GetPresignedUrl(string key, int expiryMinutes = 60)
    {
        // noneed async its locally on machine computation 
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }
        
        return _s3.GetPreSignedURL(new GetPreSignedUrlRequest()
        {
            BucketName = _options.Value.BucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Protocol = Protocol.HTTP
        });
        
        
    }
}