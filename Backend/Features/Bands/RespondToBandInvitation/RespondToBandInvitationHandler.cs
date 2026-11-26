using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.RespondToBandInvitation;

public class RespondToBandInvitationHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public RespondToBandInvitationHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<RespondToBandInvitationResponse>> HandleAsync(
        Guid invitationId, RespondToBandInvitationRequest request, string userId)
    {

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Result<RespondToBandInvitationResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        await _dbContext.Entry(user).Collection(au => au.UserBands).LoadAsync();

        var invitation = await _dbContext.BandInvitations.FirstOrDefaultAsync(bi => bi.Id == invitationId && bi.InvitedToUserId == userId);
        
        if (invitation == null)
        {
            return Result<RespondToBandInvitationResponse>.Failure(new Error
            {
                Code = "Band_Invitation.NotFound",
                Message = "Band invitation not found",
                Type = ErrorType.NotFound,
            });

        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Result<RespondToBandInvitationResponse>.Failure(new Error
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
            return Result<RespondToBandInvitationResponse>.Failure(new Error
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
            return Result<RespondToBandInvitationResponse>.Failure(new Error
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

        return Result<RespondToBandInvitationResponse>.Success(new RespondToBandInvitationResponse()
        {
            InvitationId = invitation.Id,
            BandId = invitation.BandId,
            Status = invitation.Status,
        });
    }
}
