using Microsoft.AspNetCore.Identity;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Uploads;

public class UploadHandler
{
    private readonly AppDbContext _dbContext;
    private readonly S3MediaStorage _s3;
    private readonly UserManager<AppUser> _userManager;
    public UploadHandler(AppDbContext dbContext,  UserManager<AppUser> userManager, S3MediaStorage s3)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _s3 = s3;
    }

    public async Task<Result<UploadResponse>> HandleAsync(UploadRequest request, string userId )
    {
        
        var user =  await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            
            return Result<UploadResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User Not Found",
                Type = ErrorType.NotFound,
            });
        }


        var maxBytes = (request.Purpose, request.FileType) switch
        {
            (UploadPurpose.Profile or UploadPurpose.Post, "image/jpg" or "image/jpeg" or "image/png") =>
                5L * 1024 * 1024,
            (UploadPurpose.Post, "audio/mp3" or "audio/wav") => 50L * 1024 * 1024,
            (UploadPurpose.Post, "video/mp4") => 100L * 1024 * 1024,
            _ => 0
        };
        if (maxBytes == 0)
        {
            
            return Result<UploadResponse>.Failure(new Error
            {
                Code = "Upload.UnsupportedType",
                Message = "File is not supported",
                Type = ErrorType.BadRequest,
            });
        }

        if (request.FileSize < 1 || request.FileSize > maxBytes)
        {
            
            return Result<UploadResponse>.Failure(new Error
            {
                Code = "Upload.SizeNotValid",
                Message = "File Size is not valid",
                Type = ErrorType.BadRequest,
            });
            

        }

        var uploadId = Guid.CreateVersion7();
        var key = $"{request.Purpose}/{uploadId}/original";
        var preSignedUrl = await _s3.GetPreSignedURLAsync(key, request.FileType);


        var mediaUpload = new MediaUpload()
        {
            Id = uploadId,
            RequestedByUserId = user.Id,
            ContentType = request.FileType,
            FileName = request.FileName,
            ObjectKey = key,
            Purpose = request.Purpose,
            Size = request.FileSize,
            Status = UploadStatus.Pending,
        };
        
        _dbContext.Add(mediaUpload);
        await _dbContext.SaveChangesAsync();

        var uploadResponse = new UploadResponse()
        {
            UploadId = uploadId,
            UploadUrl = preSignedUrl,
        };
        return Result<UploadResponse>.Success(uploadResponse);


    }
}