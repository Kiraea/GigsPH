using Microsoft.EntityFrameworkCore;
using PrjA.Infrastructure;
using PrjA.Common;

namespace PrjA.Features.Instruments;

public class GetInstrumentsHandler
{
    private readonly AppDbContext _dbContext;

    public GetInstrumentsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<GetInstrumentsResponse>>> HandleAsync()
    {
        var result = await _dbContext.Instruments.AsNoTracking().ToListAsync();

        return Result<List<GetInstrumentsResponse>>.Success(
        [
            .. result.Select((i) => new GetInstrumentsResponse
            {
                Id = i.Id,
                Name = i.Name,
            })
        ]);
    }
}
