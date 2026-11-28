import { useNavigate } from '@tanstack/react-router'
import {
    type GetGroupInvitationResponse,
    useRespondToGroupInvitation,
} from '#/queries/conversationQueries.tsx'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'

type GroupInvitationCardProps = {
    invitation: GetGroupInvitationResponse
}

export function GroupInvitationCard(props: GroupInvitationCardProps) {
    const respond = useRespondToGroupInvitation()
    const navigate = useNavigate()

    const handleRespond = (accept: boolean) => {
        respond.mutate(
            {
                invitationId: props.invitation.invitationId,
                request: { accept },
            },
            {
                onSuccess: (data) => {
                    if (data.status === 'Accepted' && data.conversationId) {
                        navigate({
                            to: '/conversations/$conversationId',
                            params: { conversationId: data.conversationId },
                        })
                    }
                },
            },
        )
    }

    return (
        <div className="flex flex-col gap-2 rounded-xl bg-background-medium p-3">
            <p className="font-semibold truncate">
                {props.invitation.groupName}
            </p>

            {respond.isError && (
                <p className="text-xs text-red-500">
                    {getApiErrorMessage(respond.error)}
                </p>
            )}

            <div className="flex gap-2">
                <button
                    type="button"
                    disabled={respond.isPending}
                    onClick={() => handleRespond(true)}
                    className="flex-1 rounded-lg bg-purple-400 py-1 text-sm text-foreground-high disabled:opacity-50"
                >
                    {respond.isPending ? '...' : 'Accept'}
                </button>
                <button
                    type="button"
                    disabled={respond.isPending}
                    onClick={() => handleRespond(false)}
                    className="flex-1 rounded-lg bg-background-low py-1 text-sm disabled:opacity-50"
                >
                    Decline
                </button>
            </div>
        </div>
    )
}
