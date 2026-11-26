import { useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { X } from 'lucide-react'
import { getUsersQuery } from '#/queries/userQueries.tsx'
import { useSendInvitation } from '#/queries/bandQueries.tsx'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'

type UserModalProps = {
    onClose: () => void
    bandId: string
    bandName: string
    memberIds: string[]
}

export function UserModal(props: UserModalProps) {
    const [search, setSearch] = useState('')
    const [debouncedSearch, setDebouncedSearch] = useState('')
    const [pendingIds, setPendingIds] = useState<string[]>([])
    const [invitedIds, setInvitedIds] = useState<string[]>([])

    const usersResponse = useQuery(getUsersQuery(debouncedSearch))
    const sendInvitation = useSendInvitation()

    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearch(search)
        }, 400)

        return () => {
            clearTimeout(timer)
        }
    }, [search])

    const users = usersResponse.data?.users ?? []

    const handleInvite = (userId: string) => {
        if (
            pendingIds.includes(userId) ||
            invitedIds.includes(userId) ||
            props.memberIds.includes(userId)
        ) {
            return
        }

        setPendingIds((prev) => [...prev, userId])
        sendInvitation.mutate(
            {
                bandId: props.bandId,
                request: { toId: userId },
            },
            {
                onSuccess: () => {
                    setInvitedIds((prev) => [...prev, userId])
                },
                onSettled: () => {
                    setPendingIds((prev) => prev.filter((a) => a !== userId))
                },
            },
        )
    }

    return (
        <div
            className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4"
            onClick={props.onClose}
        >
            <div
                className="flex w-full max-w-md flex-col gap-4 rounded-2xl bg-background-high p-6 text-foreground-high shadow-lg"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex items-center justify-between">
                    <h2 className="text-lg font-semibold">
                        Invite to {props.bandName}
                    </h2>
                    <button
                        type="button"
                        onClick={props.onClose}
                        aria-label="Close"
                    >
                        <X />
                    </button>
                </div>

                <input
                    type="text"
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    placeholder="Search musicians by name..."
                    className="w-full px-3 py-2"
                />

                {sendInvitation.isError && (
                    <p className="text-sm text-red-500">
                        {getApiErrorMessage(sendInvitation.error)}
                    </p>
                )}

                <div className="flex max-h-72 flex-col gap-2 overflow-y-auto">
                    {usersResponse.isPending && <div>Loading ....</div>}

                    {usersResponse.isError && (
                        <div className="text-sm text-red-500">
                            {getApiErrorMessage(usersResponse.error)}
                        </div>
                    )}

                    {!usersResponse.isPending &&
                        !usersResponse.isError &&
                        users.length === 0 && <div>No Users Found</div>}

                    {users.map((user) => {
                        const isMember = props.memberIds.includes(user.id)
                        const isPending = pendingIds.includes(user.id)
                        const isInvited = invitedIds.includes(user.id)

                        let buttonText = 'Invite'
                        if (isMember) buttonText = 'Member'
                        else if (isInvited) buttonText = 'Invited'
                        else if (isPending) buttonText = 'Inviting...'

                        return (
                            <div
                                key={user.id}
                                className="flex items-center justify-between rounded-xl bg-background-medium p-3"
                            >
                                <div>
                                    <p className="font-medium">
                                        {user.displayName}
                                    </p>
                                    {user.instruments.length > 0 && (
                                        <p className="text-xs text-foreground-low">
                                            {user.instruments
                                                .map((ins) => ins.name)
                                                .join(', ')}
                                        </p>
                                    )}
                                </div>
                                <button
                                    type="button"
                                    onClick={() => handleInvite(user.id)}
                                    disabled={
                                        isInvited || isPending || isMember
                                    }
                                    className="rounded-full bg-purple-300 px-3 py-1 text-sm text-neutral-900 disabled:opacity-50"
                                >
                                    {buttonText}
                                </button>
                            </div>
                        )
                    })}
                </div>
            </div>
        </div>
    )
}
