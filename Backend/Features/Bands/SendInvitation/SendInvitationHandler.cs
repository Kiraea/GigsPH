using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;
using PrjA.Infrastructure.Email;

namespace PrjA.Features.Bands.SendInvitation;

public class SendInvitationHandler
{
    private readonly AppDbContext _dbContext;
    private readonly EmailSender _emailSender;
    
    private readonly UserManager<AppUser> _userManager;
    public SendInvitationHandler(AppDbContext dbContext, UserManager<AppUser> userManager, EmailSender emailSender)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public async Task<Result<SendInvitationResponse>> HandleAsync(SendInvitationRequest request, string userId)
    {


        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<SendInvitationResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        
        var toUser = await _userManager.FindByIdAsync(request.ToId);
        if (toUser == null)
        {
            return Result<SendInvitationResponse>.Failure(new Error
            {
                Code = "Band_Invitation.RecipientNotFound",
                Message = "The user you want to invite was not found.",
                Type = ErrorType.NotFound,
            });
        }
        
        var band = await _dbContext.Bands.FirstOrDefaultAsync(b => b.Id == request.BandId);

        if (band == null)
        {
            return Result<SendInvitationResponse>.Failure(new Error
            {
                Code = "Band.NotFound",
                Message = "Band not found",
                Type = ErrorType.NotFound,
            });
        }
        
        
        
        /* only do this approach if the usermemberships are already loaded if not the then just use
         _dbContext_userbands cause the comparison is load -> search vs search (_dbcontext_userbands)
        which search wins
         
        // check if leader in that specific band
        await _dbContext.Entry(user).Collection(u => u.UserBands).LoadAsync();
        var userBand= user.UserBands.(b => b.BandId == request.BandId && b.Role == BandRole.Leader);
        if (userBand == null) 
        {
            throw new Exception("Not a leader // not a part of band");
        }
        */
        
        var isLeader = await _dbContext.UserBands.AnyAsync(b => b.UserId == userId && b.BandId == band.Id && b.Role == BandRole.Leader);
        if (!isLeader)
        {
            return Result<SendInvitationResponse>.Failure(new Error
            {
                Code = "Band.NotLeader",
                Message = "Only a band leader can send invitations.",
                Type = ErrorType.Forbidden,
            });
        }
        
        var alreadyMember = await _dbContext.UserBands.AnyAsync(ub => ub.UserId == toUser.Id && ub.BandId == band.Id);
        if (alreadyMember)
        {
            return Result<SendInvitationResponse>.Failure(new Error
            {
                Code = "Band.AlreadyMember",
                Message = "This user is already a member of the band.",
                Type = ErrorType.BadRequest,
            });
        }

        var alreadyInvited = await _dbContext.BandInvitations.AnyAsync(bi => bi.BandId == request.BandId &&
                                                                             bi.InvitedToUserId == request.ToId
                                                                             && bi.Status == InvitationStatus.Pending);
        
        if (alreadyInvited)
        {
            return Result<SendInvitationResponse>.Failure(new Error
            {
                Code = "Band_Invitation.AlreadyPending",
                Message = "This user already has a pending invitation to the band.",
                Type = ErrorType.BadRequest,
            });
        }
          var invitation = new BandInvitation()
        {
            BandId = request.BandId,
            InvitedByUserId = userId,
            InvitedToUserId = request.ToId,
            Status = InvitationStatus.Pending
        };
          
        _dbContext.BandInvitations.Add(invitation);
        
        await _dbContext.SaveChangesAsync();


        

        var email = new Email() {
            ToEmail = toUser.Email!, // the reason why ! instead of an if check ius because email already has a check inside emailSender eitherway if there wasnt then put if
            ToDisplayName = toUser.DisplayName!,
            InputBody= $"You have been invited to join {band.Name}",
            InputSubject= $"You have been invited to join {band.Name}",
        };

 
        
        await _emailSender.BuildMessage(email).SendEmailAsync();
        return Result<SendInvitationResponse>.Success(new SendInvitationResponse()
        {
            InvitationId = invitation.Id,
            BandId = band.Id,
            ToId = toUser.Id,
            Status = InvitationStatus.Pending
        });

    }
}
