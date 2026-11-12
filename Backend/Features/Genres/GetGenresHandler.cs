using Microsoft.EntityFrameworkCore;
using PrjA.Infrastructure;

namespace PrjA.Features.Genres;

public class GetGenresHandler
{

    private readonly AppDbContext _dbContext;
    public GetGenresHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GetGenresResponse>> HandleAsync()
    {
        
        var result = await _dbContext.Genres.AsNoTracking().ToListAsync();


        return
        [
            .. result.Select((g) => new GetGenresResponse
            {
                Id = g.Id,
                Name = g.Name,
            })
        ];

    }
}