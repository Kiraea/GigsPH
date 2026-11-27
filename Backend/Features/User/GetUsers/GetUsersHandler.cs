using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Features.Genres;
using PrjA.Infrastructure;

namespace PrjA.Features.User.GetUsers;

public class GetUsersHandler
{
    private readonly AppDbContext _dbContext;

    public GetUsersHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetUsersResponse>> HandleAsync(string? search, int page, int pageSize)
    {
        // size 10
        // page 1 reuqest

        page = page < 1 ? 1 : page;
        pageSize = Math.Clamp(pageSize, 1, 50);

        var query = _dbContext.Users.AsNoTracking().Where(au => au.DisplayName != null && au.IsOnboarded);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var trimmed = search.Trim();
            
            query = query.Where(au => EF.Functions.ILike(au.DisplayName!, $"%{trimmed}%"));
        }
        
        var totalCount = await query.CountAsync();
        
            


        var users = await  query
            .OrderBy(au => au.DisplayName)
            .ThenBy(au => au.Id)
            .Skip((page - 1) * pageSize).Take(pageSize).Select(au => new PublicUserDto
            {
                DisplayName = au.DisplayName ?? "Unknown User",
                Id = au.Id,
                Instruments =  au.UserInstruments.Where(aui => aui.Instrument != null).OrderByDescending(aui => aui.isMain).ThenBy(aui => aui.Instrument!.Name).Select(aui => new PublicUserInstrumentDto()
                {
                    Id = aui.InstrumentId,
                    IsMain = aui.isMain,
                    Name = aui.Instrument!.Name // should this be ! ?
                }).ToList(),
                Genres = au.Genres.Select(aug => new GenreDto()
                {
                    Id = aug.Id,
                    Name = aug.Name
                }).ToList(),
            }).ToListAsync();

        
        
        // 53 pages
        // 4 * 10 = 40
        // 5 * 50 = 50
        // 6 * 60 = 60
        
        var hasMore = page * pageSize < totalCount;

        return Result<GetUsersResponse>.Success(new GetUsersResponse()
        {
            Page = page,
            HasMore = hasMore,
            PageSize = pageSize,
            TotalCount = totalCount,
            Users = users
        });
            
        

    }
}
