using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Features.Genres;
using PrjA.Features.Instruments;
using PrjA.Features.Posts.GetPost;
using PrjA.Infrastructure;
using StackExchange.Redis;

namespace PrjA.Features.Posts.GetPublicPosts;

public class GetPublicPostsHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IConnectionMultiplexer _redis;
    private readonly string _mediaBaseUrl;
    private readonly UserManager<AppUser> _userManager;

    public GetPublicPostsHandler(AppDbContext dbContext, UserManager<AppUser> userManager, IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _redis = redis;

        var mediaBaseUrl = configuration["Media:BaseUrl"];
        if (!Uri.TryCreate(mediaBaseUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp &&
             uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                "Media:BaseUrl must be an absolute HTTP or HTTPS URL.");
        }
        _mediaBaseUrl = mediaBaseUrl!.TrimEnd('/');

    }

    
    
    // public user can access
    public async Task<Result<GetPublicPostsResponse>> HandleAsync(
        GetPublicPostsRequest request, string? userId)
    {

        
        var db = _redis.GetDatabase();
        List<Guid>? compatiblePosts = null;
        AppUser? user = null;

        if (request.Cursor == null)
        {
            if (userId != null)
            {
                user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return Result<GetPublicPostsResponse>.Failure(new Error
                    {
                        Code = "User.NotExist",
                        Message = "User Not Found",
                        Type = ErrorType.NotFound
                    });

                }
                // check if theres already another sessionFeed that is not expired 
                // another check to see if the user has already finished the posts -> a function to append in redis more posts not sure how it would look like specificallydo i reuse buildpersonalizedfeed or create a new function-> 
                // a function t
                if()
                    compatiblePosts = await HelperBuildPersonalizedFeedAsync(user);
          
            }
            else
            {
                compatiblePosts = await HelperBuildAnonymousFeedAsync();
            }
        
            var compatiblePostsRedis = compatiblePosts.Select(ps => (RedisValue)ps.ToString()).ToArray();

            
            
            
            Guid sessionId = Guid.CreateVersion7();
            var nextIndex = compatiblePosts.Count() + 1;

            string cursor = $"{sessionId}:{nextIndex}";
            
            // key format feed:{sessionId}:post
            RedisKey postKey = $"feed:{sessionId}:post";
            
            // key format feed:{sessionId}:owner
            RedisKey ownerKey = $"feed:{sessionId}:owner";
            
            // check the !
            RedisValue ownerValue = userId != null ? user!.Id : "anonymous";
            
            var transaction = db.CreateTransaction();
            
            transaction.SetAddAsync(postKey, compatiblePostsRedis);
            transaction.ListRightPushAsync(ownerKey, ownerValue);

            transaction.KeyExpireAsync(postKey, new TimeSpan(2, 0, 0));
            transaction.KeyExpireAsync(ownerKey, new TimeSpan(2, 0, 0));


            bool committed = await transaction.ExecuteAsync();

            if (!committed)
            {
                throw new Exception("cannot save feed session in redis");
            }
            
                
            //Store REDIS RANKING
                
                
                
            //Make send actual post
            var sessionPosts = await _dbContext.Posts.Include(p => p.Genres).Include(p => p.Instruments).Include(p => p.MediaUploads).Include(p => p.Likes).Where(p => compatiblePosts.Contains(p.Id)).ToListAsync();


            PostDto[] returnSessionPost =
            [
                .. sessionPosts.Select(sp => new PostDto()
                {
                    Id = sp.Id,
                    Title = sp.Title,
                    BandId = sp.BandId,
                    CreatedByUserId = sp.CreatedByUserId,
                    Description = sp.Description,
                    IsLiked = sp.Likes.Any(l => l.UserId == user.Id),
                    LikedCount = sp.Likes.Count,
                    Genres = sp.Genres.Select(g => new GetGenresResponse
                    {
                        Id = g.Id,
                        Name = g.Name
                    }).ToList(),
                    Instruments = sp.Instruments.Select(i => new GetInstrumentsResponse()
                    {
                        Id = i.Id,
                        Name = i.Name,
                    }).ToList(),

                }).Take(10)
            ];

            return Result<GetPublicPostsResponse>.Success(new GetPublicPostsResponse()
            {
                Posts = returnSessionPost,
                Cursor = cursor,
            });
        }
        else
        {
            // returns just cursor
            // construct key first
            // need to figure out if theres next values in the redis and return the next
            // if theres no more candidates in redis (construct an ewone? idk how)
            // if not user verify is cursor bleonging to him
            string[] cursorValues = request.Cursor.Split(':');
            string FeedSessionId = cursorValues[0];
            string nextIndex = cursorValues[1];

            RedisKey keyPostConstruct = $"feed:{FeedSessionId}:post";
            RedisKey keyOwnerConstruct = $"feed:{FeedSessionId}:owner";
            
            // get owner first
            // if anynoymous no need to verify is cursor is belonging to him
            var owner = await db.StringGetAsync(keyOwnerConstruct);
            
            if (owner.IsNull)
            {
                return Result<GetPublicPostsResponse>.Failure(new Error
                {
                    Code = "Feed.Expired",
                    Message = "This feed session is unavailable. Start a new feed.",
                    Type = ErrorType.BadRequest
                });
            }
            
            
            
            
            if (owner.ToString() != "anonymous")
            {
                // logged in
                if (userId != null)
                {
                    user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return Result<GetPublicPostsResponse>.Failure(new Error
                        {
                            Code = "User.NotExist",
                            Message = "User Not Found",
                            Type = ErrorType.NotFound
                        });

                    }

                    if (owner.ToString() != user.Id)
                    {
                        
                        return Result<GetPublicPostsResponse>.Failure(new Error
                        {
                            Code = "User.Mismatch",
                            Message = "User Misamtch to request",
                            Type = ErrorType.BadRequest
                        });
                    }
                }
                else
                {
                    throw new Exception("mismatch between data send and authorization");
                }

            }
            
            string?[] postIds = db.SetMembers(keyPostConstruct).ToStringArray();
            if (postIds == null)
            {
                throw new("no posts avaialble");
            }

            if (!int.TryParse(nextIndex, out var intIndex) ||  intIndex < 0)
            {
                return Result<GetPublicPostsResponse>.Failure(new Error
                {
                    Code = "Feed.InvalidCursor",
                    Message = "The feed position is invalid.",
                    Type = ErrorType.BadRequest
                });
            }
            
            
            
            
            

     
            
        }
        
        
      
    }

    public async Task<List<Guid>>HelperBuildPersonalizedFeedAsync(AppUser user)
    {
          // genre match mean any thing in the userScore if he watches 1 of the genre then include that genre as an option
            // score doent matter

            // we do constraint no dups at a later so no need to check
            // * genres that users have watch atleast 1 time
            var candidateGenreIds = await _dbContext.UserScoreGenres.Where(usg => usg.UserId == user.Id && usg.Score > 0 ).Select(usg => usg.GenreId).ToListAsync ();
            
            
            var candidateInstrumentIds = await _dbContext.UserScoreInstruments.Where(usg => usg.UserId== user.Id && usg.Score > 0  ).Select(usg => usg.InstrumentId).ToListAsync();
            
            
            var candidatePublisherIds = await _dbContext.UserScorePublishers.Where(usp => usp.UserId == user.Id  && usp.Score > 0).Select(usg => usg.PublisherId).ToListAsync();

            var maxCandidateGenrePosts = 100;
            var maxCandidatePublisherPosts = 50;
            var maxCandidateInstrumentPosts = 100;
            
            
            var GenrePosts = await _dbContext.Posts.Include(p => p.Genres).Where(p => p.Genres.Any(g => candidateGenreIds.Contains(g.Id)))
                .Take(maxCandidateGenrePosts).ToListAsync();
            
            
            
            var InstrumentPosts = await _dbContext.Posts.Include(i => i.Instruments)
                .Where(p => p.Instruments.Any(i => candidateInstrumentIds.Contains(i.Id)))
                .Take(maxCandidateInstrumentPosts).ToListAsync();
            
            
            var PublisherPosts = await _dbContext.Posts
                .Where(p => (p.UserId != null && candidatePublisherIds.Contains(p.UserId)) ||
                            (p.BandId != null && candidatePublisherIds.Contains(p.BandId.Value.ToString())))
                .Take(maxCandidatePublisherPosts).ToListAsync();
            
            

            var candidatePosts = GenrePosts.Concat(InstrumentPosts).Concat(PublisherPosts).DistinctBy(p => p.Id)
                .ToList();




            /* can do this but not perforamnt
             *
             var publisherScore = _dbContext.UserScorePublishers.Where(usp =>
                (post.UserId != null && usp.PublisherId == post.UserId) ||
                (post.BandId != null && usp.PublisherId == post.BandId.ToString())).Select(usp => usp.Score).FirstOrDefaultAsync();
             *
             */

            var dictionaryPublisherScore = await _dbContext.UserScorePublishers
                .Where(usp => user.Id == usp.UserId && candidatePublisherIds.Contains(usp.PublisherId))
                .ToDictionaryAsync(d => d.PublisherId, d => d.Score);

            var dictionaryGenreScore = await _dbContext.UserScoreGenres
                .Where(usg => usg.UserId== user.Id && candidateGenreIds.Contains(usg.GenreId))
                .ToDictionaryAsync(g => g.GenreId, g => g.Score);
            var dictionaryInstrumentScore = await _dbContext.UserScoreInstruments
                .Where(usi => usi.UserId== user.Id && candidateInstrumentIds.Contains(usi.InstrumentId))
                .ToDictionaryAsync(usi => usi.InstrumentId, usi => usi.Score);
            
            
            var postAndScore = new List<(Guid PostId, double Score)>();
            foreach (var post in candidatePosts)
            {
                var publisherId = post.UserId ?? post.BandId?.ToString();
                if (publisherId is null)
                {
                    throw new Exception("Post does not have a publisher");
                }

                double rawPublisherScore = dictionaryPublisherScore.GetValueOrDefault(publisherId );
                double totalPublisherScore = rawPublisherScore / (rawPublisherScore + 10);
                double totalGenreScore = 0;
                foreach (var genre in post.Genres)
                {
                    totalGenreScore += dictionaryGenreScore.GetValueOrDefault(genre.Id);
                }

                double totalInstrumentScore = 0;
                foreach (var instrument in post.Instruments)
                {
                    totalInstrumentScore += dictionaryInstrumentScore.GetValueOrDefault(instrument.Id);
                }

                double totalLabelCount = post.Instruments.Count + post.Genres.Count;
                double rawTotalLabelScore = totalLabelCount == 0 ? 0 : (totalGenreScore + totalInstrumentScore)  / totalLabelCount;
                double totalLabelScore = rawTotalLabelScore / (rawTotalLabelScore + 10);
                
                
                double totalScore = totalLabelCount == 0 ? (totalPublisherScore * 0.35) / 0.35 : ((totalLabelScore * 0.30) + (totalPublisherScore * 0.35)) / .65;

                postAndScore.Add((post.Id, totalScore));
            }

            return postAndScore.OrderBy(ps => ps.Score).ThenBy(ps => ps.PostId).Select(ps => ps.PostId).ToList();
    }
    
    public Task<List<Guid>> HelperBuildAnonymousFeedAsync()
    {
        List<Guid> template =
        [Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
        ];
        // not done
        return Task.FromResult(template);
    }

}

