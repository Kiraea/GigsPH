import type { GetConversationResponse } from '#/queries/conversationQueries.tsx'
import { ConvertDateTimeToReadableSummary } from '#/utils/generalUtils.ts'
import { Link } from '@tanstack/react-router'

export function ConversationCard({
    conversation,
}: {
    conversation: GetConversationResponse
}) {
    return (
        <Link
            className="flex justify-between "
            to="/conversations/$conversationId"
            params={{ conversationId: conversation.conversationId }}
            activeProps={{ className: 'bg-background-medium' }}
        >
            <div className="flex-col flex-1 flex min-w-0">
                <p className="truncate font-bold text-md">
                    {conversation.conversationName}
                </p>

                {conversation.latestSender != null &&
                    conversation.latestMessage != null && (
                        <p className="truncate font-bold text-sm">
                            {conversation.latestSender}:{' '}
                            {conversation.latestMessage}
                        </p>
                    )}
            </div>

            <div className="shrink-0 w-20">
                {conversation.latestDate != null && (
                    <time className="font-medium text-sm">
                        {ConvertDateTimeToReadableSummary(
                            conversation.latestDate,
                        )}
                    </time>
                )}
            </div>
        </Link>
    )
}
// shgrink-0 dont steal width from me its locked
// min-w-0 wrap the parent if the child are too much
// this only not extend the parent but not the child
