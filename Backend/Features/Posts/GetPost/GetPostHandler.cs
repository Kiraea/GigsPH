using Microsoft.AspNetCore.Authorization;
using PrjA.Common;
using PrjA.Infrastructure;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PrjA.Domain.Entities;

namespace PrjA.Features.Posts.GetPost;

public class GetPostHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public GetPostHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    
    // no need post userId everyone can view
    public async Task<Result<GetPostResponse>> HandleAsync(
        GetPostRequest request)
    {
        throw new NotImplementedException();



    }
}

