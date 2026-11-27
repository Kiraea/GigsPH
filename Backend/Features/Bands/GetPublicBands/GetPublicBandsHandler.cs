using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.GetPublicBands;

public class GetPublicBandsHandler
{
    private readonly AppDbContext _dbContext;

    public GetPublicBandsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetPublicBandsResponse>> HandleAsync(GetPublicBandsRequest request)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var query = _dbContext.Bands
            .AsNoTracking()
            .Where(b => b.UserBands.Any());

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(b => EF.Functions.ILike(b.Name, $"%{search}%"));
        }

        var totalCount = await query.CountAsync();

        var bands = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new PublicBandDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                MemberCount = b.UserBands.Count,
                CreatedAt = b.CreatedAt,
            })
            .ToListAsync();

        return Result<GetPublicBandsResponse>.Success(new GetPublicBandsResponse
        {
            Bands = bands,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasMore = page * pageSize < totalCount,
        });
    }
}
