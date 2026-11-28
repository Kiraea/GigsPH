import { createFileRoute } from '@tanstack/react-router'
import { useState } from 'react'
import {
    getConversationMessagesQuery,
    getConversationsQuery,
    getGroupDetailsQuery,
} from '#/queries/conversationQueries.tsx'
import { useInfiniteQuery, useQuery } from '@tanstack/react-query'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'
import { GroupDetails } from '#/components/GroupDetails.tsx'

export const Route = createFileRoute(
    '/_app/_authenticated/conversations/$conversationId',
)({
    component: RouteComponent,
})

function RouteComponent() {
    const [openConvoTab, setOpenConvoTab] = useState<boolean>(false)
    const { conversationId } = Route.useParams()

    const conversationQueries = useQuery(getConversationsQuery())
    const conversation = conversationQueries.data?.conversations.find(
        (c) => c.conversationId === conversationId,
    )

    const isGroup = conversation?.conversationType === 'Group'

    const groupDetailsQuery = useQuery({
        ...getGroupDetailsQuery(conversationId),
        enabled: isGroup,
    })

    const groupDetails = groupDetailsQuery.data
    const members = groupDetails?.userGroupChatDetails ?? []

    const { data, isPending, hasNextPage, isFetchingNextPage, error, isError } =
        useInfiniteQuery(
            getConversationMessagesQuery({ conversationId: conversationId }),
        )

    return (
        <div className="h-full flex">
            <div className="flex-1 flex flex-col gap-5">
                <div className="flex justify-between border-b border-foreground-low">
                    <div className="flex flex-col p-5">
                        {conversationQueries.isPending && (
                            <div>Loading ...</div>
                        )}

                        {conversationQueries.isError && (
                            <div>
                                {getApiErrorMessage(conversationQueries.error)}
                            </div>
                        )}
                        {!conversationQueries.isPending &&
                            !conversationQueries.isError &&
                            !conversation && <div>No Conversation Found</div>}

                        {!conversationQueries.isPending &&
                            !conversationQueries.isError &&
                            conversation && (
                                <>
                                    <label className="font-bold text-2xl">
                                        {conversation?.conversationName}
                                    </label>
                                    <div className="flex gap-2">
                                        {isGroup && groupDetails && (
                                            <>
                                                <p>
                                                    {members.length}
                                                    Members
                                                </p>
                                                <p>*</p>
                                            </>
                                        )}
                                        {conversation && (
                                            <p className="first-letter:uppercase">
                                                {conversation.conversationType}{' '}
                                                Chat
                                            </p>
                                        )}
                                    </div>
                                </>
                            )}
                    </div>
                    {isGroup && groupDetails && (
                        <div className="flex">
                            <button
                                className="p-2"
                                onClick={() => setOpenConvoTab(!openConvoTab)}
                            >
                                Toggle
                            </button>
                        </div>
                    )}
                </div>

                <div className="overflow-y-auto">
                    adsa dsadsa ddsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                </div>
                <div className="flex"></div>
            </div>

            {openConvoTab && groupDetails && (
                <GroupDetails groupDetails={groupDetails} />
            )}
        </div>
    )
}
