import { axiosInstance } from '#/axiosInstance.tsx'
import {
    queryOptions,
    useMutation,
    useQueryClient,
} from '@tanstack/react-query'

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

export type InvitationResponseRequest = {
    status: 'Accepted' | 'Rejected'
}

export type InvitationStatus = 'Accepted' | 'Rejected' | 'Pending' | 'Cancelled'

export type InvitationResponseResult = {
    invitationId: string
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

export type UpdateBandMemberRoleRequest = {
    role: BandRole
}

export type UpdateBandMemberRoleResponse = {
    bandId: string
    targetUserId: string
    targetRole: BandRole
    actingUserId: string
    actingRole: BandRole
}

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

export type LeaveBandResponse = {
    id: string
}

const leaveBand = async (bandId: string): Promise<LeaveBandResponse> => {
    const response = await axiosInstance.post<LeaveBandResponse>(
        `/bands/${bandId}/leave`,
    )
    return response.data
}

export const useLeaveBand = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: leaveBand,
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['bands'] })
        },
    })
}

const updateBandMemberRole = async ({
    bandId,
    targetUserId,
    request,
}: {
    bandId: string
    targetUserId: string
    request: UpdateBandMemberRoleRequest
}): Promise<UpdateBandMemberRoleResponse> => {
    const response = await axiosInstance.patch<UpdateBandMemberRoleResponse>(
        `/bands/${bandId}/members/${targetUserId}/role`,
        request,
    )
    return response.data
}

export const useUpdateBandMemberRole = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: updateBandMemberRole,
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['bands'] })
        },
    })
}

const getMyBandInvitations = async (): Promise<
    GetMyBandInvitationResponse[]
> => {
    const response = await axiosInstance.get('/bands/invitations')
    return response.data
}

export const getMyBandInvitationQuery = () => {
    return queryOptions<GetMyBandInvitationResponse[]>({
        queryKey: ['bands', 'invitations'],
        queryFn: getMyBandInvitations,
    })
}

const getMyBands = async (): Promise<GetMyBandResponse[]> => {
    const response = await axiosInstance.get('/bands/me')
    return response.data
}

export const getMyBandsQuery = () => {
    return queryOptions<GetMyBandResponse[]>({
        queryKey: ['bands', 'me'],
        queryFn: getMyBands,
    })
}

const CreateBand = async (
    request: CreateBandRequest,
): Promise<CreateBandResponse> => {
    const response = await axiosInstance.post<CreateBandResponse>(
        '/bands',
        request,
    )
    return response.data
}

export const useCreateBand = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: CreateBand,
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['bands'] })
        },
    })
}

const invitationResponse = async ({
    request,
    invitationId,
}: {
    request: InvitationResponseRequest
    invitationId: string
}): Promise<InvitationResponseResult> => {
    const response = await axiosInstance.post<InvitationResponseResult>(
        `/bands/invitations/${invitationId}/response`,
        request,
    )
    return response.data
}

export const useInvitationResponse = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: invitationResponse,
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['bands'] })
        },
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
