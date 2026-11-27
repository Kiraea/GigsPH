using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.GetMyBands;

public class GetMyBandsHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public GetMyBandsHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<List<GetMyBandResponse>>> HandleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<List<GetMyBandResponse>>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var bands = await _dbContext.UserBands
            .AsNoTracking()
            .Where(ub => ub.UserId == user.Id && ub.Band != null)
            .OrderBy(ub => ub.Role)
            .ThenByDescending(ub => ub.Band!.CreatedAt)
            .Select(ub => new GetMyBandResponse
            {
                Id = ub.Band!.Id,
                Name = ub.Band.Name,
                Description = ub.Band.Description,
                MyRole = ub.Role,
                MemberCount = ub.Band.UserBands.Count,
                CreatedAt = ub.Band.CreatedAt,
                Members = ub.Band.UserBands
                    .OrderBy(m => m.Role)
                    .ThenBy(m => m.CreatedAt)
                    .Select(m => new BandMemberDto
                    {
                        UserId = m.UserId,
                        DisplayName = m.User!.DisplayName ?? m.User.UserName ?? "Musician",
                        Role = m.Role,
                        Instruments = m.User.UserInstruments
                            .Where(ui => ui.Instrument != null)
                            .OrderByDescending(ui => ui.isMain)
                            .ThenBy(ui => ui.Instrument!.Name)
                            .Select(ui => new BandMemberInstrumentDto
                            {
                                Id = ui.InstrumentId,
                                Name = ui.Instrument!.Name,
                                IsMain = ui.isMain,
                            })
                            .ToList(),
                    })
                    .ToList(),
            })
            .ToListAsync();

        return Result<List<GetMyBandResponse>>.Success(bands);
    }
}
