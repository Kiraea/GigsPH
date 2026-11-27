using PrjA.Features.Genres;

namespace PrjA.Features.User.GetUsers;

public class GetUsersResponse
{
    public List<PublicUserDto> Users { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasMore { get; set; }
}

public class PublicUserDto
{
    public required string Id { get; set; }
    public required string DisplayName { get; set; }
    public List<PublicUserInstrumentDto> Instruments { get; set; } = [];
    public List<GenreDto> Genres { get; set; } = [];
}

public class PublicUserInstrumentDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsMain { get; set; }
}
