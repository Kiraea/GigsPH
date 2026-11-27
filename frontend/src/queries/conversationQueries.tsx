import { axiosInstance } from '#/axiosInstance.tsx'
import {
    infiniteQueryOptions,
    queryOptions,
    useMutation,
    useQueryClient,
} from '@tanstack/react-query'

// ── Enums ──
export type ConversationType = 'Direct' | 'Group'
export type DirectRequestStatus = 'Accepted' | 'Pending' | 'Rejected'
export type GroupChatInvitationStatus = 'Accepted' | 'Pending' | 'Declined'

// ── Core Conversation DTOs ──
export type GetConversationResponse = {
    conversationId: string
    conversationName: string
    conversationType: ConversationType
    directRequestStatus: DirectRequestStatus | null
    canAcceptRequest: boolean
    latestMessage: string | null
    latestSender: string | null
    latestDate: string | null
}

export type GetConversationsResponse = {
    conversations: GetConversationResponse[]
}

export type MessageDto = {
    id: string
    text: string
    senderUserId: string
    senderDisplayName: string
    clientMessageId: string
    createdAt: string
}

export type GetConversationMessagesResponse = {
    conversationId: string
    messages: MessageDto[]
    cursor: string | null
}

export type SendMessageRequest = {
    text: string
    clientMessageId: string
}

export type SendMessageResponse = {
    conversationId: string
    messageId: string
    clientMessageId: string
    senderUserId: string
    text: string
    senderDisplayName: string
    createdAt: string
}

export type StartDirectConversationRequest = {
    recipientUserId: string
    text: string
    clientMessageId: string
}

export type StartDirectConversationResponse = {
    conversationId: string
}
export type RespondToDirectRequestResponse = {
    conversationId: string
}

// ── Group Conversation DTOs ──
export type CreateGroupConversationRequest = {
    name: string
    invitedUserIds: string[]
}

export type CreateGroupConversationResponse = {
    conversationId: string
}

export type UserGroupChatDetail = {
    userId: string
    displayName: string
}

export type GetGroupDetailsResponse = {
    conversationId: string
    name: string
    userGroupChatDetails: UserGroupChatDetail[]
    isOwner: boolean
}

export type GetGroupInvitationResponse = {
    invitationId: string
    groupName: string
}

export type InviteGroupMemberRequest = {
    userId: string
}

export type InviteGroupMemberResponse = {
    conversationId: string
    invitationId: string
}

export type RespondToGroupInvitationRequest = {
    accept: boolean
}

export type RespondToGroupInvitationResponse = {
    invitationId: string
    conversationId: string | null
    status: GroupChatInvitationStatus
}

export type UpdateGroupRequest = {
    name: string
}

export type UpdateGroupResponse = {
    conversationId: string
    name: string
}

export type RemoveGroupMemberResponse = {
    conversationId: string
    removedUserId: string
}

export type TransferGroupOwnershipRequest = {
    newOwnerUserId: string
}

export type TransferGroupOwnershipResponse = {
    conversationId: string
    ownerUserId: string
}

export type LeaveGroupResponse = {
    conversationId: string
}

// ── Queries ──

const getConversationMessages = async ({
    conversationId,
    cursor,
}: {
    conversationId: string
    cursor: string | null
}): Promise<GetConversationMessagesResponse> => {
    const response = await axiosInstance.get<GetConversationMessagesResponse>(
        `/conversations/${conversationId}/messages`,
        {
            params: { cursor: cursor ?? undefined },
        },
    )
    return response.data
}

export const getConversationMessagesQuery = ({
    conversationId,
}: {
    conversationId: string
}) => {
    return infiniteQueryOptions({
        queryKey: ['conversations', 'messages', conversationId],
        queryFn: ({ pageParam }) =>
            getConversationMessages({
                conversationId: conversationId,
                cursor: pageParam,
            }),
        initialPageParam: null as string | null,
        getNextPageParam: (oldData) => {
            return oldData.cursor
        },
    })
}

const getConversations = async () => {
    const response =
        await axiosInstance.get<GetConversationsResponse>('/conversations')
    return response.data
}

export const getConversationsQuery = () => {
    return queryOptions({
        queryKey: ['conversations', 'list'],
        queryFn: getConversations,
    })
}

const getGroupDetails = async (conversationId: string) => {
    const response = await axiosInstance.get<GetGroupDetailsResponse>(
        `/conversations/groups/${conversationId}`,
    )
    return response.data
}

export const getGroupDetailsQuery = (conversationId: string) => {
    return queryOptions({
        queryKey: ['conversations', 'groups', 'details', conversationId],
        queryFn: () => getGroupDetails(conversationId),
    })
}

const getGroupInvitations = async () => {
    const response = await axiosInstance.get<GetGroupInvitationResponse[]>(
        `/conversations/groups/invitations`,
    )

    return response.data
}

export const getGroupInvitationsQuery = () => {
    return queryOptions({
        queryKey: ['conversations', 'groups', 'invitations'],
        queryFn: getGroupInvitations,
    })
}

// ── Mutations ──

const sendMessage = async ({
    conversationId,
    request,
}: {
    conversationId: string
    request: SendMessageRequest
}): Promise<SendMessageResponse> => {
    const response = await axiosInstance.post<SendMessageResponse>(
        `/conversations/${conversationId}/messages`,
        request,
    )
    return response.data
}

export const useSendMessage = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: sendMessage,
        onSuccess: async (response) => {
            await queryClient.invalidateQueries({
                queryKey: ['conversations', 'messages', response.conversationId],
            })
            await queryClient.invalidateQueries({
                queryKey: ['conversations', 'list'],
            })
        },
    })
}

const startDirectConversation = async (
    request: StartDirectConversationRequest,
): Promise<StartDirectConversationResponse> => {
    const response = await axiosInstance.post<StartDirectConversationResponse>(
        '/conversations/direct',
        request,
    )
    return response.data
}

export const useStartDirectConversation = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: startDirectConversation,
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: ['conversations', 'list'],
            })
        },
    })
}

const createGroupConversation = async (
    request: CreateGroupConversationRequest,
) => {
    const response = await axiosInstance.post<CreateGroupConversationResponse>(
        '/conversations/groups',
        request,
    )
    return response.data
}

export const useCreateGroupConversation = () => {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: createGroupConversation,
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: ['conversations', 'list'],
            })
        },
    })
}

const inviteGroupMember = async ({
    request,
    conversationId,
}: {
    request: InviteGroupMemberRequest
    conversationId: string
}): Promise<InviteGroupMemberResponse> => {
    const response = await axiosInstance.post<InviteGroupMemberResponse>(
        `/conversations/groups/${conversationId}/invitations`,
        request,
    )

    return response.data
}

export const useInviteGroupMember = () => {
    return useMutation({
        mutationFn: inviteGroupMember,
    })
}

const leaveGroup = async (conversationId: string) => {
    const response = await axiosInstance.post<LeaveGroupResponse>(
        `/conversations/groups/${conversationId}/leave`,
    )
    return response.data
}

export const useLeaveGroup = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: leaveGroup,
        onSuccess: async (response) => {
            queryClient.removeQueries({
                queryKey: [
                    'conversations',
                    'messages',
                    response.conversationId,
                ],
            })
            queryClient.removeQueries({
                queryKey: [
                    'conversations',
                    'groups',
                    'details',
                    response.conversationId,
                ],
            })
            await queryClient.invalidateQueries({
                queryKey: ['conversations', 'list'],
            })
        },
    })
}

const removeGroupMember = async ({
    conversationId,
    userId,
}: {
    conversationId: string
    userId: string
}) => {
    const response = await axiosInstance.delete<RemoveGroupMemberResponse>(
        `/conversations/groups/${conversationId}/members/${userId}`,
    )
    return response.data
}

export const useRemoveGroupMember = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: removeGroupMember,
        onSuccess: async (response) => {
            await queryClient.invalidateQueries({
                queryKey: [
                    'conversations',
                    'groups',
                    'details',
                    response.conversationId,
                ],
            })
        },
    })
}

const respondToGroupInvitation = async ({
    request,
    invitationId,
}: {
    request: RespondToGroupInvitationRequest
    invitationId: string
}) => {
    const response = await axiosInstance.post<RespondToGroupInvitationResponse>(
        `/conversations/groups/invitations/${invitationId}/response`,
        request,
    )
    return response.data
}

export const useRespondToGroupInvitation = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: respondToGroupInvitation,
        onSuccess: async (response) => {
            if (response.status === 'Accepted') {
                await queryClient.invalidateQueries({
                    queryKey: ['conversations', 'list'],
                })
            }
            await queryClient.invalidateQueries({
                queryKey: ['conversations', 'groups', 'invitations'],
            })
        },
    })
}

const transferGroupOwnership = async ({
    request,
    conversationId,
}: {
    request: TransferGroupOwnershipRequest
    conversationId: string
}) => {
    const response = await axiosInstance.put<TransferGroupOwnershipResponse>(
        `/conversations/groups/${conversationId}/owner`,
        request,
    )

    return response.data
}

export const useTransferGroupOwnership = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: transferGroupOwnership,
        onSuccess: async (response) => {
            await queryClient.invalidateQueries({
                queryKey: [
                    'conversations',
                    'groups',
                    'details',
                    response.conversationId,
                ],
            })
        },
    })
}

const updateGroup = async ({
    conversationId,
    request,
}: {
    conversationId: string
    request: UpdateGroupRequest
}) => {
    const response = await axiosInstance.patch<UpdateGroupResponse>(
        `/conversations/groups/${conversationId}`,
        request,
    )
    return response.data
}

export const useUpdateGroup = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: updateGroup,
        onSuccess: async (response) => {
            await queryClient.invalidateQueries({
                queryKey: [
                    'conversations',
                    'groups',
                    'details',
                    response.conversationId,
                ],
            })

            await queryClient.invalidateQueries({
                queryKey: ['conversations', 'list'],
            })
        },
    })
}

const respondToDirectRequest = async (conversationId: string) => {
    const response = await axiosInstance.post<RespondToDirectRequestResponse>(
        `/conversations/${conversationId}/accept`,
    )
    return response.data
}

export const useRespondToDirectRequest = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: respondToDirectRequest,
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: ['conversations', 'list'],
            })
        },
    })
}
