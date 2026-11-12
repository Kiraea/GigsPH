using Microsoft.EntityFrameworkCore;
using PrjA.Infrastructure;

namespace PrjA.Features.Instruments;

public class GetInstrumentsHandler
{
    private readonly AppDbContext _dbContext;

    public GetInstrumentsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GetInstrumentsResponse>> HandleAsync()
    {
        var result = await _dbContext.Instruments.AsNoTracking().ToListAsync();

        return
        [
            .. result.Select((i) => new GetInstrumentsResponse
            {
                Id = i.Id,
                Name = i.Name,
            })
        ];
    }
}
