import { createFileRoute, Outlet } from '@tanstack/react-router'
import { useState } from 'react'
import { ConversationCard } from '#/components/conversationCard.tsx'

export const Route = createFileRoute('/_app/_authenticated/conversations')({
    component: RouteComponent,
})

export type ConversationType = 'direct' | 'group'
export type DirectRequestStatus = 'Accepted' | 'Pending' | 'Rejected'

export type ConversationResponse = {
    conversationId: number
    conversationName: string
    conversationType: ConversationType
    directRequestStatus: DirectRequestStatus | null
    canAcceptRequest: boolean
    latestMessage: string | null
    latestSender: string | null
    latestDate: string | null
}
function RouteComponent() {
    const dummyConversationData: ConversationResponse[] = [
        {
            conversationId: 101,
            conversationName: 'Maya Santos',
            conversationType: 'direct',
            directRequestStatus: 'Accepted',
            canAcceptRequest: false,
            latestMessage: 'I can send the guitar demo tonight.',
            latestSender: 'Maya Santos',
            latestDate: '2026-11-23T03:15:00Z',
        },
        {
            conversationId: 102,
            conversationName: 'Sunday Jazz Jam',
            conversationType: 'group',
            directRequestStatus: null,
            canAcceptRequest: false,
            latestMessage:
                'Soundcheck is at 4. Bring the chart for the new song.',
            latestSender: 'Nico Reyes',
            latestDate: '2026-11-22T11:40:00Z',
        },
        {
            conversationId: 103,
            conversationName: 'Ari Mendoza',
            conversationType: 'direct',
            directRequestStatus: 'Pending',
            canAcceptRequest: true,
            latestMessage:
                'Hey! Are you still looking for a bassist for Friday?',
            latestSender: 'Ari Mendoza',
            latestDate: '2026-11-22T08:05:00Z',
        },
        {
            conversationId: 104,
            conversationName: 'Rooftop Set Rehearsal',
            conversationType: 'group',
            directRequestStatus: null,
            canAcceptRequest: false,
            latestMessage: 'Let’s run the ending twice before we pack up.',
            latestSender: 'Lea Cruz',
            latestDate: '2026-11-21T12:20:00Z',
        },
        {
            conversationId: 105,
            conversationName: 'Sam Rivera',
            conversationType: 'direct',
            directRequestStatus: 'Accepted',
            canAcceptRequest: false,
            latestMessage: 'Thanks for the recording!',
            latestSender: 'Sam Rivera',
            latestDate: '2026-11-20T07:30:00Z',
        },
        {
            conversationId: 106,
            conversationName: 'Weekend Recording',
            conversationType: 'group',
            directRequestStatus: null,
            canAcceptRequest: false,
            latestMessage: null,
            latestSender: null,
            latestDate: null,
        },
    ]

    // false for show accepted
    // true for pending
    let [toggleChat, setToggleChat] = useState<boolean>(false)

    let acceptedPosts = dummyConversationData.filter(
        (val) => !val.canAcceptRequest,
    )
    let pendingPosts = dummyConversationData.filter(
        (val) => val.canAcceptRequest,
    )

    return (
        <div className="grid grid-cols-[280px_minmax(0,1fr)]  ">
            <div className="flex flex-col gap-2 border-1 border-gray-200 h-screen ">
                <h1 className="text-2xl font-bold">Messages</h1>
                <div className="flex justify-around bg-background-medium p-1 mx-2 rounded-2xl">
                    <button
                        className={`flex-1 p-2 rounded-2xl ${toggleChat ? 'bg-background-high' : 'bg-background-medium'}`}
                        onClick={() => setToggleChat(true)}
                    >
                        Chat
                    </button>
                    <button
                        className={`flex-1 p-2 rounded-2xl ${toggleChat ? 'bg-background-medium' : 'bg-background-high'}`}
                        onClick={() => setToggleChat(false)}
                    >
                        Request
                    </button>
                </div>

                {toggleChat ? (
                    <div className="flex flex-col gap-3 min-h-0 overflow-y-auto">
                        {acceptedPosts.length != 0 &&
                            acceptedPosts.map((conversation) => (
                                <ConversationCard conversation={conversation} />
                            ))}
                    </div>
                ) : (
                    <div className="flex flex-col gap-3">
                        {pendingPosts.length != 0 &&
                            pendingPosts.map((conversation) => (
                                <ConversationCard conversation={conversation} />
                            ))}
                    </div>
                )}
            </div>
            <div className="h-screen">
                <Outlet />
            </div>
        </div>
    )
}
