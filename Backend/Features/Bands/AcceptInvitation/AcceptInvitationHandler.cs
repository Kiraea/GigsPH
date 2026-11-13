using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
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

    public async Task<Result<AcceptInvitationResponse>> HandleAsync(
        AcceptInvitationRequest request, string userId)
    {

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Result<AcceptInvitationResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        await _dbContext.Entry(user).Collection(au => au.UserBands).LoadAsync();

        var invitation = await _dbContext.BandInvitations.FirstOrDefaultAsync(bi => bi.Id == request.InvitationId && bi.InvitedToUserId == userId);
        
        if (invitation == null)
        {
            return Result<AcceptInvitationResponse>.Failure(new Error
            {
                Code = "Band_Invitation.NotFound",
                Message = "Band invitation not found",
                Type = ErrorType.NotFound,
            });

        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Result<AcceptInvitationResponse>.Failure(new Error
            {
                Code = "Band_Invitation.NotPending",
                Message = "Band Invitation is no longer pending",
                Type = ErrorType.BadRequest,
            });
        }

        // check if he already part of the band
        var isInBand = await _dbContext.UserBands.Include(ub => ub.Band).AnyAsync(b => b.UserId == user.Id && b.BandId == invitation.BandId);
        if (isInBand)
        {
            return Result<AcceptInvitationResponse>.Failure(new Error
            {
                Code = "Band.AlreadyMember",
                Message = "Already in this specific Band",
                Type = ErrorType.BadRequest,
            });
        }

        /*
         * request.Status == InvitationStatus.Accepted || alt
    request.Status == InvitationStatus.Rejected;

         */
        if (request.Status != InvitationStatus.Accepted && request.Status != InvitationStatus.Rejected)
        {
            return Result<AcceptInvitationResponse>.Failure(new Error
            {
                Code = "Band_Invitation.InvalidStatus",
                Message = "Status should be Accepted or Rejected",
                Type = ErrorType.BadRequest,
            });
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

        return Result<AcceptInvitationResponse>.Success(new AcceptInvitationResponse()
        {
            InvitationId = invitation.Id,
            BandId = invitation.BandId,
            Status = invitation.Status,
        });







    }
}
