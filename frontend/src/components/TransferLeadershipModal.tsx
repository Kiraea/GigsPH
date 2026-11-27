import { useState } from 'react'
import { X } from 'lucide-react'
import {
    type GetMyBandResponse,
    useUpdateBandMemberRole,
} from '#/queries/bandQueries.tsx'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'

type TransferLeadershipModalProps = {
    band: GetMyBandResponse
    onClose: () => void
}

export function TransferLeadershipModal(props: TransferLeadershipModalProps) {
    const [selectedUserId, setSelectedUserId] = useState('')
    const updateRole = useUpdateBandMemberRole()

    const eligibleMembers = props.band.members.filter(
        (member) => member.role !== 'Leader',
    )

    const handleTransfer = () => {
        if (!selectedUserId) {
            return
        }

        updateRole.mutate(
            {
                bandId: props.band.id,
                targetUserId: selectedUserId,
                request: { role: 'Leader' },
            },
            {
                onSuccess: () => {
                    props.onClose()
                },
            },
        )
    }

    return (
        <div
            className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center"
            onClick={() => !updateRole.isPending && props.onClose()}
        >
            <div
                className="bg-background-high w-full max-w-md p-6 text-foreground-high rounded-2xl flex flex-col gap-4"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex items-center justify-between">
                    <h2 className="text-lg font-bold">
                        Transfer Leadership - {props.band.name}
                    </h2>
                    <button
                        type="button"
                        onClick={props.onClose}
                        disabled={updateRole.isPending}
                    >
                        <X className="h-5 w-5" />
                    </button>
                </div>

                <p className="text-sm text-foreground-low">
                    Choose a member to become the new leader. You will become a
                    regular member.
                </p>

                {eligibleMembers.length === 0 ? (
                    <div className="text-sm text-foreground-low">
                        No other members to transfer leadership to.
                    </div>
                ) : (
                    <div className="flex flex-col gap-2 max-h-60 overflow-y-auto">
                        {eligibleMembers.map((member) => {
                            const isSelected = selectedUserId === member.userId
                            return (
                                <button
                                    key={member.userId}
                                    type="button"
                                    disabled={updateRole.isPending}
                                    onClick={() =>
                                        setSelectedUserId(member.userId)
                                    }
                                    className={`flex items-center justify-between p-3 rounded-xl border text-left ${
                                        isSelected
                                            ? 'border-purple-400 bg-background-medium'
                                            : 'border-foreground-low/30 bg-background-low'
                                    }`}
                                >
                                    <span className="font-semibold">
                                        {member.displayName}
                                    </span>
                                    <span className="text-xs text-foreground-low">
                                        {member.instruments
                                            .map((ins) => ins.name)
                                            .join(', ')}
                                    </span>
                                </button>
                            )
                        })}
                    </div>
                )}

                {updateRole.isError && (
                    <div className="text-sm text-red-400">
                        {getApiErrorMessage(updateRole.error)}
                    </div>
                )}

                <div className="flex justify-end gap-2">
                    <button
                        type="button"
                        onClick={props.onClose}
                        disabled={updateRole.isPending}
                        className="bg-background-medium px-4 py-2 rounded-xl text-foreground-low"
                    >
                        Cancel
                    </button>
                    <button
                        type="button"
                        onClick={handleTransfer}
                        disabled={updateRole.isPending || !selectedUserId}
                        className="bg-purple-400 px-4 py-2 rounded-xl text-foreground-high disabled:opacity-50"
                    >
                        {updateRole.isPending
                            ? 'Transferring...'
                            : 'Transfer Leadership'}
                    </button>
                </div>
            </div>
        </div>
    )
}
