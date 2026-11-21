using PrjA.Domain.Entities;

namespace PrjA.Features.MusicListings.GetMusicListingApplications;

public class GetMusicListingApplicationsResponse
{
    public IReadOnlyList<MusicListingSlotApplicantsDto> Slots { get; init; } = [];
}

public class MusicListingSlotApplicantsDto
{
    public Guid SlotId { get; init; }
    public Guid InstrumentId { get; init; }
    public IReadOnlyList<MusicListingApplicantDto> Applicants { get; init; } = [];
}

public class MusicListingApplicantDto
{
    public Guid ApplicationId { get; init; }
    public required string ApplicantUserId { get; init; }
    public required string DisplayName { get; init; }
    public string? Note { get; init; }
    public MusicListingApplicationStatus Status { get; init; }
}
