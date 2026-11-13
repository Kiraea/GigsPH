using Amazon.S3;
using Amazon.S3.Model;

namespace PrjA.Infrastructure;

public class S3MediaStorage
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucketName;

    public S3MediaStorage(IAmazonS3 s3, string bucketName)
    {
        _s3 = s3;
        _bucketName = bucketName;
    }

    public async Task<string> GetPreSignedURLAsync(string key, string contentType)
    {

        GetPreSignedUrlRequest request = new GetPreSignedUrlRequest()
        {
            BucketName = _bucketName,
            Expires = DateTime.UtcNow.AddMinutes(5),
            ContentType = contentType,
            Key = key,
            Protocol = Protocol.HTTP, // HTTPS if real amazon floci is HTTP
            Verb = HttpVerb.PUT // reason why is default is .GET and it means get a presigned url to access a file
        };
        var result = await _s3.GetPreSignedURLAsync(request);
        return result;
        
    }
    
    // function that checks if every image needed to check is uploaded returns bool
    public async Task<bool> CheckIfImagesUploadedAndValidated(string key, string contentType, long fileSize)
    {
        try
        {
            var result = await _s3.GetObjectMetadataAsync(_bucketName, key);
            return result.ContentLength == fileSize && string.Equals(contentType, result.ContentType, StringComparison.OrdinalIgnoreCase);

        }
        catch (AmazonS3Exception e)
        {
            if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            else
            {
                throw;
            }
        }


    }
        

    
}