using PrjA.Infrastructure;

namespace PrjA.Features.Bands.UpdateBandMemberRole;

public class UpdateBandMemberRoleHandler
{
    private readonly AppDbContext _dbContext;

    public UpdateBandMemberRoleHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UpdateBandMemberRoleResponse> HandleAsync(
        UpdateBandMemberRoleRequest request, string userId)
    {
        // TODO: Implement updating a band member's role.
        throw new NotImplementedException();
    }
}