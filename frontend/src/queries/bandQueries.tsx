import { axiosInstance } from '#/axiosInstance.tsx'
import { useMutation } from '@tanstack/react-query'

export type CreateBandRequest = {
    name: string
    description: string | null
}

export type CreateBandResponse = {
    id: string
    name: string
    description: string | null
    role: BandRole
}

export type BandRole = 'Leader' | 'Member'

export type AcceptInvitationRequest = {
    Status: string
}

export type InvitationStatus = 'Accepted' | 'Rejected' | 'Pending' | 'Cancelled'

export type AcceptInvitationResponse = {
    invitationId: number
    status: string
    bandId: string
}

export type SendInvitationRequest = {
    toId: string
}
export type SendInvitationResponse = {
    invitationId: string
    bandId: string
    toId: string
    status: string
}
export type UpdateBandMemberRoleRequest = {}
export type UpdateBandMemberRoleResponse = {}

export type BandMemberInstrument = {
    id: string
    name: string
    isMain: boolean
}

export type BandMember = {
    userId: string
    displayName: string
    role: BandRole
    instruments: BandMemberInstrument[]
}

export type GetMyBandResponse = {
    id: string
    name: string
    description: string | null
    myRole: BandRole
    memberCount: number
    members: BandMember[]
    createdAt: string
}

export type GetMyBandInvitationResponse = {
    invitationId: string
    bandId: string
    bandName: string
    bandDescription: string | null
    memberCount: number
    invitedByUserId: string
    invitedByDisplayName: string
    createdAt: string
}

export type PublicBand = {
    id: string
    name: string
    description: string | null
    memberCount: number
    createdAt: string
}

export type GetPublicBandsResponse = {
    bands: PublicBand[]
    page: number
    pageSize: number
    totalCount: number
    hasMore: boolean
}

const CreateBand = async (
    request: CreateBandRequest,
): Promise<CreateBandResponse> => {
    const response = await axiosInstance.post<CreateBandResponse>(
        '/api/bands',
        request,
    )
    return response.data
}

const useCreateBand = () => {
    return useMutation({
        mutationFn: CreateBand,
    })
}

const AcceptInvitation = ({
    request,
    bandId,
    invitationId,
}: {
    request: AcceptInvitationRequest
    bandId: string
    invitationId: string
}): Promise<AcceptInvitationResponse> => {
    const response = await axiosInstance.post(
        `/bands/${bandId}/invitations/${invitationId}/response`,
        request,
    )
    return response.data
}

const useAcceptInvitation = () => {
    return useMutation({
        mutationFn: AcceptInvitation,
        // put next time invalidate here
    })
}

const sendInvitation = async ({
    bandId,
    request,
}: {
    bandId: string
    request: SendInvitationRequest
}): Promise<SendInvitationResponse> => {
    const response = await axiosInstance.post<SendInvitationResponse>(
        `/bands/${bandId}/invitations`,
        request,
    )
    return response.data
}

export const useSendInvitation = () => {
    return useMutation({
        mutationFn: sendInvitation,
    })
}
