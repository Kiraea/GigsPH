using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Features.Posts.GetPost;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.GetPublicPosts;

public class GetPublicPostsHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public GetPublicPostsHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    
    
    // public user can access
    public async Task<Result<GetPublicPostsResponse>> HandleAsync(
        GetPublicPostsRequest request, string? userId)
    {


        AppUser? user = null;
        if (userId != null)
        {
            user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Result<GetPostResponse>.Failure(new Error
                {
                    Code = "User.NotExist",
                    Message = "User Not Found",
                    Type = ErrorType.NotFound
                });

            }
            // genre match mean any thing in the userScore if he watches 1 of the genre then include that genre as an option
            // score doent matter

            // we do constraint no dups at a later so no need to check
            // * genres that users have watch atleast 1 time
            var candidateGenreIds = _dbContext.UserScoreGenres.Select(usg => usg.GenreId).ToList();
            var candidateInstrumentIds = _dbContext.UserScoreInstruments.Select(usg => usg.InstrumentId).ToList();
            var candidatePublisherIds = _dbContext.UserScorePublishers.Select(usg => usg.PublisherId).ToList();

            var maxCandidateGenrePosts = 100;
            var maxCandidatePublisherPosts = 50;
            var maxCandidateInstrumentPosts = 100;
            var GenrePosts = await _dbContext.Posts.Where(p => p.Genres.Any(g => candidateGenreIds.Contains(g.Id)))
                .Take(maxCandidateGenrePosts).ToListAsync();
            var InstrumentPosts = await _dbContext.Posts
                .Where(p => p.Instruments.Any(i => candidateInstrumentIds.Contains(i.Id)))
                .Take(maxCandidateInstrumentPosts).ToListAsync();
            var PublisherPosts = await _dbContext.Posts
                .Where(p => (p.UserId != null && candidatePublisherIds.Contains(p.UserId)) ||
                            (p.BandId != null && candidatePublisherIds.Contains(p.BandId.Value.ToString())))
                .Take(maxCandidatePublisherPosts).ToListAsync();

            var candidatePosts = GenrePosts.Concat(InstrumentPosts).Concat(PublisherPosts).DistinctBy(p => p.Id)
                .ToList();


            var postAndScore = new List<(Guid PostId, double Score)>();


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
                .Where(usg => usg.Userid == user.Id && candidateGenreIds.Contains(usg.GenreId))
                .ToDictionaryAsync(g => g.GenreId, g => g.Score);
            var dictionaryInstrumentScore = await _dbContext.UserScoreInstruments
                .Where(usi => usi.Userid == user.Id && candidateInstrumentIds.Contains(usi.InstrumentId))
                .ToDictionaryAsync(usi => usi.InstrumentId, usi => usi.Score);
            
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

                postAndScore.Add((post.Id, totalScore);



            }
            
            // lets do 0.30 for genres 0.35 for Publisher 0.35 for freshness
        }
        
        
        
        
        // return here
    }

}

