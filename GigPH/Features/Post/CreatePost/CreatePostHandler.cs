using GigPH.Domain;
using GigPH.Infrastructure;

namespace GigPH.Features.Post.CreatePost;



public class CreatePostHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IS3Service _s3;
     
    public CreatePostHandler(AppDbContext dbContext, IS3Service s3)
    {
        _dbContext = dbContext;
        _s3 = s3;
    }

    public async Task<CreatePostResponse> HandleAsync(CreatePostRequest request)
    {
        Media? media = null;
        if (request.File != null)
        {
            var ext = Path.GetExtension(request.File.FileName);
            
            media = new Media
            {
                FileSize = request.File.Length,
                Name = request.File.FileName,
                OwnerId = request.UserId,
                OwnerType = OwnerType.Post,
                Type = request.File.ContentType
            };
            
            // u can do this cause it only vlaidates when u do save changes async so safe
            var key =  $"Post/{request.UserId}/{media.Id}/ext";
            media.Key = key;
            var stream = request.File.OpenReadStream();

            var response = await _s3.UploadAsync(stream, key, request.File.ContentType);
             _dbContext.Medias.Add(media);
        }

        var post = new Domain.Post
        {
            Description = request.Description!,
            UserId = request.UserId,
            MediaId = media?.Id,
            Title = request.Title!,
            Id = Guid.CreateVersion7()
        };

        _dbContext.Posts.Add(post);
        await _dbContext.SaveChangesAsync();

        return new CreatePostResponse()
        {
            Description = request.Description!,
            FileName = media?.Name,
            Id = post.Id,
            MediaType = media?.Type,
            Title = request.Title!,
            UserId = request.UserId
        };
    }
    
}