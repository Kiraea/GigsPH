// ── Enums ──
import { axiosInstance } from '#/axiosInstance.tsx'
import { queryOptions } from '@tanstack/react-query'

export type MusicListingApplicationStatus = 'Pending' | 'Accepted' | 'Withdrawn'

// ── Shared DTOs ──
export type MusicListingGenreDto = {
    id: string
    name: string
}

export type MusicListingSlotDto = {
    id: string
    instrumentId: string
    isFilled: boolean
    note: string | null
}

export type MusicListingDto = {
    id: string
    title: string
    description: string | null
    genre: MusicListingGenreDto | null
    city: string
    startsAt: string
    endsAt: string
    expiresAt: string
    slots: MusicListingSlotDto[]
}

// ── GetMusicListings ──
export type GetMusicListingsRequest = {
    genreId?: string
    instrumentId?: string
    city?: string
    startsAfter?: string
    page?: number
    pageSize?: number
}

export type GetMusicListingsResponse = {
    posts: MusicListingDto[]
}

// ── CreateMusicListing ──
export type MusicListingSlotRequest = {
    instrumentId: string
    note?: string | null
}

export type CreateMusicListingRequest = {
    title: string
    description: string
    genreId: string | null
    city: string
    startsAt: string
    endsAt: string
    expiresAt: string
    musicListingSlotDto: MusicListingSlotRequest[]
}

export type CreateMusicListingResponse = {
    musicListingId: string
}

// ── ApplyToMusicListingSlot ──
export type ApplyToMusicListingSlotRequest = {
    note?: string | null
}

export type ApplyToMusicListingSlotResponse = {
    applicationId: string
    slotId: string
}

// ── GetMusicListingApplications ──
export type MusicListingApplicantDto = {
    applicationId: string
    applicantUserId: string
    displayName: string
    note: string | null
    status: MusicListingApplicationStatus
}

export type MusicListingSlotApplicantsDto = {
    slotId: string
    instrumentId: string
    applicants: MusicListingApplicantDto[]
}

export type GetMusicListingApplicationsResponse = {
    slots: MusicListingSlotApplicantsDto[]
}

// ── AcceptMusicListingApplicant ──
export type AcceptMusicListingApplicantResponse = {
    applicationId: string
    slotId: string
}

// ── CancelMusicListingApplication ──
export type CancelMusicListingApplicationResponse = {
    applicationId: string
    slotId: string
}

// ── Queries ──

/*

    public Guid? GenreId { get; init; }
    public Guid? InstrumentId { get; init; }
    public string? City { get; init; }
    public DateTime? StartsAfter { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

        
 */
const getMusicListings = async ({
    genreId,
    instrumentId,
    city,
    startsAfter,
    page = 1,
    pageSize = 20,
}: {
    genreId?: string
    instrumentId?: string
    city?: string
    startsAfter?: string
    page: number
    pageSize: number
}) => {
    const response = await axiosInstance.get<GetMusicListingsResponse>(
        '/music-listings',
        {
            params: {
                genreId: genreId ?? undefined,
                instrumentId: instrumentId ?? undefined,
                city: city ?? undefined,
                startsAfter: startsAfter ?? undefined,
                page: page,
                pageSize: pageSize,
            },
        },
    )
    return response.data
}

export const getMusicListingsQuery = async ({
    genreId,
    instrumentId,
    city,
    startsAfter,
    page = 1,
    pageSize = 20,
}: {
    genreId?: string
    instrumentId?: string
    city?: string
    startsAfter?: string
    page: number
    pageSize: number
}) => {
    return queryOptions({
        queryKey: ['music-listings'],
        queryFn: () =>
            getMusicListings({
                genreId,
                instrumentId,
                city,
                startsAfter,
                page,
                pageSize,
            }),
    })
}

// ── Mutations ──
