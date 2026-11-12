using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.AcceptInvitation;

public class AcceptInvitationHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public AcceptInvitationHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<AcceptInvitationResponse> HandleAsync(
        AcceptInvitationRequest request, string userId)
    {

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        await _dbContext.Entry(user).Collection(au => au.UserBands).LoadAsync();

        var invitation = await _dbContext.BandInvitations.FirstOrDefaultAsync(bi => bi.Id == request.InvitationId && bi.InvitedToUserId == userId);
        
        if (invitation == null)
        {
            throw new Exception("Invitation not found");
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            throw new Exception("Invitation is not processed as its not pending");
        }

        // check if he already part of the band
        var isInBand = await _dbContext.UserBands.Include(ub => ub.Band).AnyAsync(b => b.UserId == user.Id && b.BandId == invitation.BandId);
        if (isInBand)
        {
            throw new Exception("already part of band");
        }

        /*
         * request.Status == InvitationStatus.Accepted || alt
    request.Status == InvitationStatus.Rejected;

         */
        if (request.Status != InvitationStatus.Accepted && request.Status != InvitationStatus.Rejected)
        {
            throw new Exception("request Status must be accepted or rejected");
        }

        if (request.Status ==  InvitationStatus.Accepted)
        {
            var userBand = new UserBand()
            {
                BandId = invitation.BandId,
                Role = BandRole.Member,
                UserId = user.Id,
                User = user,
            };
            user.UserBands.Add(userBand);
            invitation.Status = InvitationStatus.Accepted;
            
            
        }
        else if (request.Status == InvitationStatus.Rejected)
        {
            
            invitation.Status = InvitationStatus.Rejected;
        }
        
        
        await _dbContext.SaveChangesAsync();

        return new AcceptInvitationResponse()
        {
            InvitationId = invitation.Id,
            BandId = invitation.BandId,
            Status = invitation.Status,
        };







    }
}
