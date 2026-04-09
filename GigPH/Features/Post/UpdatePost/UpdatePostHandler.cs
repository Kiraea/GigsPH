using GigPH.Domain;
using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.Post.UpdatePost;

public class UpdatePostHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IS3Service _s3;
     public UpdatePostHandler(AppDbContext dbContext, IS3Service s3)
    {
        _dbContext = dbContext;
        _s3 = s3;
    }

    public async Task<UpdatePostResponse> HandleAsync(Guid requesterId, Guid postId, UpdatePostRequest request)
    {
        var post = await _dbContext.Posts
            .Include(p => p.Media)
            .FirstOrDefaultAsync(p => p.UserId == requesterId && p.Id == postId);

        if (post == null)
            throw new Exception("Post not found.");

        if (!string.IsNullOrEmpty(request.Title)) post.Title = request.Title;
        if (!string.IsNullOrEmpty(request.Description)) post.Description = request.Description;

        string? keyToDelete = null;

        // Explicit remove requested
        if (request.RemoveMedia && post.Media != null)
        {
            keyToDelete = post.Media.Key;
            _dbContext.Medias.Remove(post.Media);
            post.Media = null;
            await _dbContext.SaveChangesAsync(); // ← save deletion first
        }

        // New file upload — also replaces any existing media not yet removed
        if (request.File != null)
        {
            if (post.Media != null)
            {
                keyToDelete = post.Media.Key;
                _dbContext.Medias.Remove(post.Media);
                post.Media = null;
                await _dbContext.SaveChangesAsync(); // ← save deletion first
            }

            var ext = Path.GetExtension(request.File.FileName);
            var media = new Media
            {
                Name = request.File.FileName,
                Type = request.File.ContentType,
                FileSize = request.File.Length,
                OwnerId = requesterId,
                OwnerType = OwnerType.Post,
            };

            media.Key = $"Post/{requesterId}/{media.Id}{ext}";

            using var stream = request.File.OpenReadStream();
            await _s3.UploadAsync(stream, media.Key, request.File.ContentType);
            _dbContext.Medias.Add(media);
            post.Media = media;
        }

        await _dbContext.SaveChangesAsync();

        // Delete from S3 only after DB is confirmed saved
        if (keyToDelete != null)
            await _s3.DeleteAsync(keyToDelete);

        return new UpdatePostResponse
        {
            Title = post.Title, // use post values, not request
            Description = post.Description,
            Id = post.Id,
            UserId = post.UserId,
            FileName = post.Media?.Name,
            MediaType = post.Media?.Type,
        };
    }
}