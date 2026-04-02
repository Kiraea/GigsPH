using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.User.UpdateProfile;


[ApiController]
public class UpdateProfileEndpoint : ControllerBase
{
    public UpdateProfileEndpoint()
    {
        
    }

    [HttpGet("{userId:Guid}")]
    public async Task<UpdateProfileResponse> UpdateProfile([FromRoute] Guid userId)
    {
        throw new NotImplementedException();
    }
}