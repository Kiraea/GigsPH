using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.CreatePost;

public class CreatePostHandler
{
    private readonly AppDbContext _dbContext;
    private readonly S3MediaStorage _s3;
    private readonly UserManager<AppUser> _userManager;
    

    public CreatePostHandler(AppDbContext dbContext, UserManager<AppUser> userManager, S3MediaStorage s3)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _s3 = s3;
    }

    public async Task<Result<CreatePostResponse>> HandleAsync(
        CreatePostRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<CreatePostResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var isInBand = false;

        if (request.BandId != null)
        {
            var valid = await _dbContext.UserBands.AnyAsync(ub => ub.BandId == request.BandId && ub.UserId == userId);

            if (!valid)
            {
                return Result<CreatePostResponse>.Failure(new Error
                {
                    Code = "Band.InvalidBandSelection",
                    Message = "Invalid band selection",
                    Type = ErrorType.BadRequest,
                });

                
            }

            isInBand = true;

        }
        
        // check if all files were uploaded succesfully use uploadedId
        // if its not uploaded all dont create post
        // if it is create post

        var uploads = await _dbContext.MediaUploads.Where(mu => request.UploadIds.Contains(mu.Id)
                                                                  && mu.RequestedByUserId == userId &&
                                                                  mu.Status == UploadStatus.Pending &&
                                                                  mu.Purpose == UploadPurpose.Post).ToListAsync();

        if (uploads.Count != request.UploadIds.Count)
        {
            
            return Result<CreatePostResponse>.Failure(new Error
            {
                Code = "Upload.InvalidUploads",
                Message = "One or more attachments are Invalid",
                Type = ErrorType.BadRequest,
            });
        }


        foreach (var upload in uploads)
        {
            var valid= await _s3.CheckIfImagesUploadedAndValidated(upload.ObjectKey, upload.ContentType, upload.Size);
            if (!valid)
            {
                return Result<CreatePostResponse>.Failure(new Error
                {
                    Code = "Post.UploadMismatch",
                    Message = "An attachment is missing or does not match its upload details.",
                    Type = ErrorType.BadRequest
                });
            }
        }

        var post = new Post()
        {
            BandId = request.BandId,
            UserId = request.BandId == null ? user.Id : null,
            CreatedByUserId = user.Id,
            Title = request.Title,
            Description = request.Description,
            MediaUploads = uploads,
        };
        foreach (var upload in uploads)
        {
            upload.Status = UploadStatus.Attached;
        }
        _dbContext.Add(post);
        await _dbContext.SaveChangesAsync();

        return Result<CreatePostResponse>.Success(new CreatePostResponse()
        {
            PostId = post.Id,
        });


    }
}

