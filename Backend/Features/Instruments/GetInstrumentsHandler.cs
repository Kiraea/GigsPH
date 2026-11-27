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

    public async Task<Result<List<InstrumentDto>>> HandleAsync()
    {
        var result = await _dbContext.Instruments.AsNoTracking().ToListAsync();

        return Result<List<InstrumentDto>>.Success(
        [
            .. result.Select((i) => new InstrumentDto
            {
                Id = i.Id,
                Name = i.Name,
            })
        ]);
    }
}
