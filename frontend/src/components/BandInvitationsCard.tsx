import {
    type GetMyBandInvitationResponse,
    type InvitationResponseRequest,
    useInvitationResponse,
} from '#/queries/bandQueries.tsx'
import { useQuery } from '@tanstack/react-query'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'

export function BandInvitationsCard({
    bandInvitation,
}: {
    bandInvitation: GetMyBandInvitationResponse
}) {
    const invitationResponse = useInvitationResponse()

    const handleInvitationResponse = (
        status: InvitationResponseRequest['status'],
    ) => {
        invitationResponse.mutate({
            invitationId: bandInvitation.invitationId,
            request: { status: status },
        })
    }

    return (
        <div className="flex flex-col bg-background-low gap-5 text-foreground-low p-2">
            <div className="flex items-center gap-2">
                <div className="h-6 w-6 rounded-full bg-background-medium flex justify-center items-center">
                    {bandInvitation.bandName.substring(0, 1)}
                </div>
                <div className="flex flex-col">
                    <h2>{bandInvitation.bandName}</h2>
                    <p>
                        Invited by{' '}
                        <span className="font-bold text-md">
                            {' '}
                            {bandInvitation.invitedByDisplayName}
                        </span>
                    </p>
                </div>
            </div>

            <hr className="border-t border-foreground-low" />

            {invitationResponse.isError && (
                <div>{getApiErrorMessage(invitationResponse.error)}</div>
            )}

            <div className="flex gap-2 p-2">
                <button
                    type="button"
                    disabled={invitationResponse.isPending}
                    onClick={() => handleInvitationResponse('Accepted')}
                    className="bg-purple-400 p-2 rounded-2xl  text-foreground-high"
                >
                    Accept
                </button>
                <button
                    type="button"
                    className="bg-background-medium p-2 rounded-2xl text-foreground-low"
                    onClick={() => handleInvitationResponse('Rejected')}
                    disabled={invitationResponse.isPending}
                >
                    Reject
                </button>
            </div>
        </div>
    )
}
