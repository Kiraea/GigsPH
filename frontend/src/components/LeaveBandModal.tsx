import { X } from 'lucide-react'
import {
    type GetMyBandResponse,
    useLeaveBand,
} from '#/queries/bandQueries.tsx'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'
import { Modal } from '#/components/Modal.tsx'

type LeaveBandModalProps = {
    band: GetMyBandResponse
    onClose: () => void
}

export function LeaveBandModal(props: LeaveBandModalProps) {
    const leaveBand = useLeaveBand()

    const isLeader = props.band.myRole === 'Leader'
    const hasOtherMembers = props.band.members.length > 1
    const mustTransferFirst = isLeader && hasOtherMembers

    const handleLeave = () => {
        leaveBand.mutate(props.band.id, {
            onSuccess: () => {
                props.onClose()
            },
        })
    }

    return (
        <Modal onClose={props.onClose} closeDisabled={leaveBand.isPending}>
            <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold">
                    {mustTransferFirst
                        ? 'Transfer Leadership First'
                        : `Leave ${props.band.name}?`}
                </h2>
                <button
                    type="button"
                    onClick={props.onClose}
                    disabled={leaveBand.isPending}
                >
                    <X className="h-5 w-5" />
                </button>
            </div>

            {mustTransferFirst ? (
                <>
                    <p className="text-sm text-foreground-low">
                        You are the leader of{' '}
                        <span className="font-semibold text-foreground-high">
                            {props.band.name}
                        </span>
                        . Transfer leadership to another member before leaving.
                    </p>

                    <div className="flex justify-end">
                        <button
                            type="button"
                            onClick={props.onClose}
                            className="bg-purple-400 px-4 py-2 rounded-xl text-foreground-high"
                        >
                            OK
                        </button>
                    </div>
                </>
            ) : (
                <>
                    <p className="text-sm text-foreground-low">
                        {hasOtherMembers
                            ? `Are you sure you want to leave ${props.band.name}?`
                            : `You are the last member of ${props.band.name}. Leaving will disband the band and cancel any pending invitations.`}
                    </p>

                    {leaveBand.isError && (
                        <div className="text-sm text-red-400">
                            {getApiErrorMessage(leaveBand.error)}
                        </div>
                    )}

                    <div className="flex justify-end gap-2">
                        <button
                            type="button"
                            onClick={props.onClose}
                            disabled={leaveBand.isPending}
                            className="bg-background-medium px-4 py-2 rounded-xl text-foreground-low"
                        >
                            Cancel
                        </button>
                        <button
                            type="button"
                            onClick={handleLeave}
                            disabled={leaveBand.isPending}
                            className="bg-red-500 px-4 py-2 rounded-xl text-foreground-high disabled:opacity-50"
                        >
                            {leaveBand.isPending ? 'Leaving...' : 'Leave Band'}
                        </button>
                    </div>
                </>
            )}
        </Modal>
    )
}
