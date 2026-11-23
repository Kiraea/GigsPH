import type { ConversationResponse } from '#/routes/_app/conversations.tsx'
import {
    ConvertDateTimeToReadable,
    ConvertDateTimeToReadableSummary,
} from '#/utils/generalUtils.ts'

export function ConversationCard({
    conversation,
}: {
    conversation: ConversationResponse
}) {
    return (
        <div className="flex justify-between ">
            <div className="flex-col flex-1 flex min-w-0">
                <label className="truncate font-bold text-md">
                    {conversation.conversationName}
                </label>

                {conversation.latestSender != null &&
                    conversation.latestMessage != null && (
                        <label className="truncate font-bold text-sm">
                            {conversation.latestSender}:{' '}
                            {conversation.latestMessage}
                        </label>
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
        </div>
    )
}
// shgrink-0 dont steal width from me its locked
// min-w-0 wrap the parent if the child are too much
// this only not extend the parent but not the child
