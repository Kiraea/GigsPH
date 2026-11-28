import { createFileRoute, Outlet } from '@tanstack/react-router'
import { useState } from 'react'
import { ConversationCard } from '#/components/conversationCard.tsx'
import {
    getConversationsQuery,
    getGroupInvitationsQuery,
} from '#/queries/conversationQueries.tsx'
import { useQuery } from '@tanstack/react-query'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'
import { StartDirectChat } from '#/components/StartDirectChat.tsx'
import { CreateGroupModal } from '#/components/CreateGroupModal.tsx'
import { GroupInvitationCard } from '#/components/GroupInvitationCard.tsx'

export const Route = createFileRoute('/_app/_authenticated/conversations')({
    component: RouteComponent,
})

function RouteComponent() {
    const conversationsQuery = useQuery(getConversationsQuery())
    const groupInvitationsQuery = useQuery(getGroupInvitationsQuery())

    const conversations = conversationsQuery.data?.conversations ?? []
    const groupInvitations = groupInvitationsQuery.data ?? []

    // true for accepted (Chat), false for pending (Request)
    let [toggleChat, setToggleChat] = useState<boolean>(true)

    let acceptedPosts = conversations.filter((val) => !val.canAcceptRequest)
    let pendingPosts = conversations.filter((val) => val.canAcceptRequest)
    const totalRequests = groupInvitations.length + pendingPosts.length

    const [isDirectModalOpen, setIsDirectModalOpen] = useState(false)
    const [isGroupModalOpen, setIsGroupModalOpen] = useState(false)

    return (
        <div className="grid grid-cols-[280px_minmax(0,1fr)]  ">
            <div className="flex flex-col gap-2 border-1 border-gray-200 h-screen ">
                <h1 className="text-2xl font-bold px-2 pt-2">Messages</h1>
                <div className="flex flex-1 min-h-0 flex-col gap-2">
                    <div className="flex justify-around bg-background-medium p-1 mx-2 rounded-2xl">
                        <button
                            type="button"
                            className={`flex-1 p-2 rounded-2xl ${toggleChat ? 'bg-background-high' : 'bg-background-medium'}`}
                            onClick={() => setToggleChat(true)}
                        >
                            Chat
                        </button>
                        <button
                            type="button"
                            className={`flex-1 p-2 rounded-2xl ${toggleChat ? 'bg-background-medium' : 'bg-background-high'}`}
                            onClick={() => setToggleChat(false)}
                        >
                            Request{totalRequests > 0 ? ` (${totalRequests})` : ''}
                        </button>
                    </div>

                    {toggleChat ? (
                        <div className="flex flex-1 min-h-0 flex-col gap-3 overflow-y-auto">
                            {conversationsQuery.isPending && (
                                <div className="px-2">Loading ...</div>
                            )}

                            {conversationsQuery.isError && (
                                <div className="px-2 text-sm text-red-500">
                                    {getApiErrorMessage(
                                        conversationsQuery.error,
                                    )}
                                </div>
                            )}

                            {!conversationsQuery.isPending &&
                                !conversationsQuery.isError &&
                                acceptedPosts.length === 0 && (
                                    <p className="px-2 text-sm text-foreground-low">
                                        No conversations yet
                                    </p>
                                )}

                            {!conversationsQuery.isPending &&
                                !conversationsQuery.isError &&
                                acceptedPosts.map((conversation) => (
                                    <ConversationCard
                                        key={conversation.conversationId}
                                        conversation={conversation}
                                    />
                                ))}
                        </div>
                    ) : (
                        <div className="flex flex-1 min-h-0 flex-col gap-4 overflow-y-auto px-2">
                            <div className="flex flex-col gap-2">
                                <p className="text-xs font-semibold uppercase text-foreground-low">
                                    Group Invitations
                                </p>

                                {groupInvitationsQuery.isPending && (
                                    <p className="text-sm text-foreground-low">
                                        Loading invitations...
                                    </p>
                                )}

                                {groupInvitationsQuery.isError && (
                                    <p className="text-sm text-red-500">
                                        {getApiErrorMessage(
                                            groupInvitationsQuery.error,
                                        )}
                                    </p>
                                )}

                                {!groupInvitationsQuery.isPending &&
                                    !groupInvitationsQuery.isError &&
                                    groupInvitations.length === 0 && (
                                        <p className="text-xs text-foreground-low">
                                            No group invitations
                                        </p>
                                    )}

                                {!groupInvitationsQuery.isPending &&
                                    !groupInvitationsQuery.isError &&
                                    groupInvitations.map((invitation) => (
                                        <GroupInvitationCard
                                            key={invitation.invitationId}
                                            invitation={invitation}
                                        />
                                    ))}
                            </div>

                            <div className="flex flex-col gap-2">
                                <p className="text-xs font-semibold uppercase text-foreground-low">
                                    Direct Requests
                                </p>

                                {conversationsQuery.isPending && (
                                    <p className="text-sm text-foreground-low">
                                        Loading requests...
                                    </p>
                                )}

                                {conversationsQuery.isError && (
                                    <p className="text-sm text-red-500">
                                        {getApiErrorMessage(
                                            conversationsQuery.error,
                                        )}
                                    </p>
                                )}

                                {!conversationsQuery.isPending &&
                                    !conversationsQuery.isError &&
                                    pendingPosts.length === 0 && (
                                        <p className="text-xs text-foreground-low">
                                            No direct requests
                                        </p>
                                    )}

                                {!conversationsQuery.isPending &&
                                    !conversationsQuery.isError &&
                                    pendingPosts.map((conversation) => (
                                        <ConversationCard
                                            key={conversation.conversationId}
                                            conversation={conversation}
                                        />
                                    ))}
                            </div>
                        </div>
                    )}
                </div>
                <div className="flex shrink-0 gap-2 p-2">
                    <button
                        type="button"
                        onClick={() => setIsDirectModalOpen(true)}
                    >
                        Start new conversation
                    </button>
                    <button
                        type="button"
                        onClick={() => setIsGroupModalOpen(true)}
                    >
                        Create Group Chat
                    </button>
                </div>

                {isDirectModalOpen && (
                    <StartDirectChat
                        onClose={() => setIsDirectModalOpen(false)}
                    />
                )}

                {isGroupModalOpen && (
                    <CreateGroupModal
                        onClose={() => setIsGroupModalOpen(false)}
                    />
                )}
            </div>
            <div className="h-screen">
                <Outlet />
            </div>
        </div>
    )
}
