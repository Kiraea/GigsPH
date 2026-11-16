using System.Runtime.InteropServices.JavaScript;
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

        //edge user sends cursor is ANONYMOUS but logged IN
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

                compatiblePosts = await HelperBuildPersonalizedFeedAsync(user, []);
          
            }
            else
            {
                compatiblePosts = await HelperBuildAnonymousFeedAsync([]);
            }
            if (compatiblePosts.Count == 0)
            {
                return Result<GetPublicPostsResponse>.Success(
                    new GetPublicPostsResponse
                    {
                        Posts = [],
                        Cursor = null
                    });
            }

            var firstBatchIds = compatiblePosts.Take(request.PageSize).ToList();
            
            var compatiblePostsRedis = compatiblePosts.Select(ps => (RedisValue)ps.ToString()).ToArray();

            
            
            
            
            Guid sessionId = Guid.CreateVersion7();

            var nextIndex = firstBatchIds.Count;

            string cursor = $"{sessionId}:{nextIndex}";
            
            // key format feed:{sessionId}:post
            RedisKey postKey = $"feed:{sessionId}:post";
            
            // key format feed:{sessionId}:owner
            RedisKey ownerKey = $"feed:{sessionId}:owner";
            
            // check the !
            RedisValue ownerValue = userId != null ? user!.Id : "anonymous";
            
            var transaction = db.CreateTransaction();
            // the reason why i need var is this returns a task even though we comittedIt it can still fail on an indnividual level
            var savePosts = transaction.ListRightPushAsync(postKey, compatiblePostsRedis);
            var saveOwner = transaction.StringSetAsync(ownerKey, ownerValue);

            var PostExpire = transaction.KeyExpireAsync(postKey, new TimeSpan(2, 0, 0));
            var OwnerExpire = transaction.KeyExpireAsync(ownerKey, new TimeSpan(2, 0, 0));


            bool committed = await transaction.ExecuteAsync();

            if (!committed)
            {
                throw new Exception("cannot save feed session in redis");
            }

            await Task.WhenAll(savePosts, saveOwner, PostExpire, OwnerExpire);
            
                
            //Store REDIS RANKING
                
                
                
            //Make send actual post
            var sessionPosts = await _dbContext.Posts.AsNoTracking().Include(p => p.Genres).Include(p => p.Instruments).Include(p => p.MediaUploads).Include(p => p.User).Include(p => p.Likes).Include(p => p.Band).Where(p => firstBatchIds.Contains(p.Id)).ToListAsync();

            var postsById = sessionPosts.ToDictionary(p => p.Id);
            

            var orderedSessionPosts = firstBatchIds.Where(id => postsById.ContainsKey(id)).Select(id => postsById[id]).ToList();

            PostDto[] returnSessionPost =
            [
                .. orderedSessionPosts.Select(sp => new PostDto()
                {
                    Id = sp.Id,
                    Title = sp.Title,
                    BandId = sp.BandId,
                    Description = sp.Description,
                    UserId = sp.UserId,
                    PublisherName = (sp.BandId != null ? sp.Band?.Name: sp.User?.DisplayName) ?? throw new InvalidOperationException("Post has no publishername"),
                    IsLiked = userId != null ? sp.Likes.Any(l => l.UserId == userId) : false,
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
                    Media = sp.MediaUploads.Select(mu => new PostMediaDto()
                    {
                        Id = mu.Id,
                        ContentType= mu.ContentType,
                        Url = $"{_mediaBaseUrl}/{mu.ObjectKey}",
                    }).ToList(),
                    

                })
            ];

            return Result<GetPublicPostsResponse>.Success(new GetPublicPostsResponse()
            {
                Posts = returnSessionPost,
                Cursor = cursor,
            });
        }
        else
        {
            bool noMoreCandidates = false;
            // returns just cursor
            // construct key first
            // need to figure out if theres next values in the redis and return the next
            // if theres no more candidates in redis (construct an ewone? idk how)
            // if not user verify is cursor bleonging to him
            string[] cursorValues = request.Cursor.Split(':');

            if (cursorValues.Length != 2)
            {
                return Result<GetPublicPostsResponse>.Failure(new Error
                {
                    Code = "Feed.InvalidCursor",
                    Message = "The feed position is invalid.",
                    Type = ErrorType.BadRequest
                });
            }
            string FeedSessionId = cursorValues[0];
            var nextIndex = cursorValues[1];

            if ( !Guid.TryParse(FeedSessionId, out var sessionId) ||
                sessionId == Guid.Empty || !int.TryParse(nextIndex, out var intIndex) || intIndex < 0)
            {
                return Result<GetPublicPostsResponse>.Failure(new Error
                {
                    Code = "Feed.InvalidCursor",
                    Message = "The feed position is invalid.",
                    Type = ErrorType.BadRequest
                });
            }

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
            var expectedOwner = userId ?? "anonymous";
            
            
            
            if (owner.ToString() != expectedOwner)
            {
                return Result<GetPublicPostsResponse>.Failure(new Error
                {
                    Code = "Feed.OwnerMismatch",
                    Message = "Start a new feed for the current viewer.",
                    Type = ErrorType.Forbidden
                });
            }
            // Still check whether the authenticated account exists.
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
            }
   
            


            var savedIds = await db.ListRangeAsync(keyPostConstruct, 0, -1);
            
            if (savedIds.Length == 0)
            {
                return Result<GetPublicPostsResponse>.Failure(new Error
                {
                    Code = "Feed.Expired",
                    Message = "This feed session is unavailable. Start a new feed.",
                    Type = ErrorType.BadRequest
                });
            }

            if (intIndex > savedIds.Length)
            {
                return Result<GetPublicPostsResponse>.Failure(new Error
                {
                    Code = "Feed.InvalidCursor",
                    Message = "The feed position is beyond the saved list.",
                    Type = ErrorType.BadRequest
                });
            }

            var excludedPostIds = savedIds
                .Select(value => Guid.Parse(value.ToString()))
                .ToList();

 
            var nextIds= await db.ListRangeAsync(keyPostConstruct, intIndex, request.PageSize + intIndex - 1);
            
            var batchIds = nextIds.Select(r => Guid.Parse(r.ToString())).ToList();
            // this means that his requesting more than what is in the sessionlist
            if ( batchIds.Count  < request.PageSize )
            {
                List<Guid> newToBeAppendPosts = [];
                if (owner.ToString() != "anonymous")
                {
                    newToBeAppendPosts = await HelperBuildPersonalizedFeedAsync(user, excludedPostIds);
                }
                else
                {
                    newToBeAppendPosts = await HelperBuildAnonymousFeedAsync(excludedPostIds);
                }

                if (newToBeAppendPosts.Count == 0)
                {
                    noMoreCandidates = true;
                }
                else
                {
                    var toRedisAppendPostsId = newToBeAppendPosts.Select(a => (RedisValue)a.ToString()).ToArray();

                
                    // p1 p2 p3 already   p4 p5 p6 not in
                    // user requests 5 new tho but no more
                    // since this is inside if it only happens 1 the append
                    // so the next request would be empty thjen it would go in the newtobeappendposts .count == 0 so exception if empty now
                    // so it handles all ithink?
 
                    
                    
                    var appendTransaction = db.CreateTransaction();

                    appendTransaction.AddCondition(Condition.KeyExists(keyPostConstruct));

                    
                    // checking if the owner has expired 
                    //tldr comparing the value gotten from key vs the owner
                    appendTransaction.AddCondition(Condition.StringEqual(keyOwnerConstruct, owner));
                    
                    // 2 request
                    // one appends first (savedIds doesnt change)
                    // but keypostconstruct(values) change already
                    appendTransaction.AddCondition(Condition.ListLengthEqual(keyPostConstruct, savedIds.Length));
                    var appendTask = appendTransaction.ListRightPushAsync((RedisKey)keyPostConstruct, toRedisAppendPostsId);
                    
                    bool appended = await appendTransaction.ExecuteAsync();

                    if (!appended)
                    {
                        return Result<GetPublicPostsResponse>.Failure(new Error
                        {
                            Code = "Feed.Retry",
                            Message = "Retry this request with the same cursor.",
                            Type = ErrorType.BadRequest
                        });
                    }
                    
                    await appendTask;

                    
                    nextIds = await db.ListRangeAsync(keyPostConstruct, intIndex, request.PageSize + intIndex - 1);

                    batchIds = nextIds.Select(i => Guid.Parse(i.ToString())).ToList();
                }
            }
            


            // redis does not overflow when reading an index that is not supopsed an item on it 
            int newIndex = intIndex + batchIds.Count;

            var getNewPosts =await _dbContext.Posts.AsNoTracking().Include(p => p.Genres).Include(p => p.Instruments).Include(p=> p.User).Include(p=> p.Band).Include(p => p.MediaUploads).Include(p => p.Likes).Where(p => batchIds.Contains(p.Id)).ToListAsync();

            var postDict = getNewPosts.ToDictionary(p => p.Id);
            
            var orderedGetNewPosts = batchIds.Where(id => postDict.ContainsKey(id)).Select(id => postDict[id]).ToList();
            
            
            PostDto[] returnSessionPost =
            [
                .. orderedGetNewPosts.Select(sp => new PostDto()
                {
                    Id = sp.Id,
                    Title = sp.Title,
                    BandId = sp.BandId,
                    Description = sp.Description,
                    UserId =  sp.UserId,
                    PublisherName = (sp.BandId != null ? sp.Band?.Name: sp.User?.DisplayName) ?? throw new InvalidOperationException("Post has no publishername"),
                    IsLiked = userId != null ? sp.Likes.Any(l => l.UserId == userId) : false,
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
                    Media = sp.MediaUploads.Select(mu => new PostMediaDto()
                    {
                        Id = mu.Id,
                        ContentType= mu.ContentType,
                        Url = $"{_mediaBaseUrl}/{mu.ObjectKey}",
                    }).ToList(),

                })
            ];
            
            var newCursor = noMoreCandidates ? null :  $"{FeedSessionId}:{newIndex}" ;
            

            return Result<GetPublicPostsResponse>.Success(new GetPublicPostsResponse()
            {
                Posts = returnSessionPost,
                Cursor = newCursor,
            });


        }
        
        
      
    }

    public async Task<List<Guid>>HelperBuildPersonalizedFeedAsync(AppUser user, List<Guid> excludedPostIds)
    {

        var eligiblePosts = _dbContext.Posts.AsNoTracking().Include(p => p.Genres).Include(p => p.Instruments)
            .Where(u => !excludedPostIds.Contains(u.Id));
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


            var genrePosts = await eligiblePosts.Where(p => p.Genres.Any(g => candidateGenreIds.Contains(g.Id)))
                .Take(maxCandidateGenrePosts).ToListAsync();
            
            
            
            var instrumentPosts = await eligiblePosts.Include(i => i.Instruments)
                .Where(p => p.Instruments.Any(i => candidateInstrumentIds.Contains(i.Id)))
                .Take(maxCandidateInstrumentPosts).ToListAsync();
            
            
            var publisherPosts = await eligiblePosts 
                .Where(p => (p.UserId != null && candidatePublisherIds.Contains(p.UserId)) ||
                            (p.BandId != null && candidatePublisherIds.Contains(p.BandId.Value.ToString())))
                .Take(maxCandidatePublisherPosts).ToListAsync();


       
            
            // things that dont match the interest of the user?
            // lets make this newly created post
            var generalPosts = await eligiblePosts.OrderByDescending(p => p.CreatedAt).ThenBy(p => p.Id).Take(100).ToListAsync();
            
            

            var candidatePosts = genrePosts.Concat(instrumentPosts).Concat(publisherPosts).Concat(generalPosts).DistinctBy(p => p.Id)
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

                double rawPublisherScore = dictionaryPublisherScore.GetValueOrDefault(publisherId);
                double totalPublisherScore = rawPublisherScore / (rawPublisherScore + 10);
                
                
                var numOfDays = DateTime.UtcNow - post.CreatedAt;
                // this is just if server is out of sync
                var days = Math.Max(0, numOfDays.TotalDays);

                double totalFreshnessScore = 1.0 / (1.0 + days);
                
                
                
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
                
                
                double totalScore = totalLabelCount == 0 ? ((totalPublisherScore * 0.35) + (totalFreshnessScore * 0.35)) / 0.7 : ((totalLabelScore * 0.30) + (totalPublisherScore * 0.35) + (totalFreshnessScore * 0.35)) / 1;

                postAndScore.Add((post.Id, totalScore));
            }

            return postAndScore.OrderByDescending(ps => ps.Score).ThenBy(ps => ps.PostId).Select(ps => ps.PostId).ToList();
    }
    
    public async Task<List<Guid>> HelperBuildAnonymousFeedAsync(List<Guid> excludedPostIds)
    {
        
        var postIds = await _dbContext.Posts.AsNoTracking().Where(p => !excludedPostIds.Contains(p.Id)).Select(p => p.Id).Take(200).ToListAsync();
        var shuffedIds = postIds.ToArray();
        Random.Shared.Shuffle(shuffedIds);
        

        return shuffedIds.ToList();

    }

}

