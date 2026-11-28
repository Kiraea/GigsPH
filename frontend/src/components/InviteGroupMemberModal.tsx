import { useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { X } from 'lucide-react'
import { getUsersQuery } from '#/queries/userQueries.tsx'
import { useInviteGroupMember } from '#/queries/conversationQueries.tsx'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'
import { Modal } from '#/components/Modal.tsx'

type InviteGroupMemberModalProps = {
    onClose: () => void
    conversationId: string
    groupName: string
    memberIds: string[]
}

export function InviteGroupMemberModal(props: InviteGroupMemberModalProps) {
    const [search, setSearch] = useState('')
    const [debouncedSearch, setDebouncedSearch] = useState('')
    const [pendingIds, setPendingIds] = useState<string[]>([])
    const [invitedIds, setInvitedIds] = useState<string[]>([])

    const usersQuery = useQuery(getUsersQuery(debouncedSearch))
    const inviteMember = useInviteGroupMember()

    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearch(search)
        }, 400)

        return () => {
            clearTimeout(timer)
        }
    }, [search])

    const users = usersQuery.data?.users ?? []

    const handleInvite = (userId: string) => {
        if (
            pendingIds.includes(userId) ||
            invitedIds.includes(userId) ||
            props.memberIds.includes(userId)
        ) {
            return
        }

        setPendingIds((prev) => [...prev, userId])
        inviteMember.mutate(
            {
                conversationId: props.conversationId,
                request: { userId },
            },
            {
                onSuccess: () => {
                    setInvitedIds((prev) => [...prev, userId])
                },
                onSettled: () => {
                    setPendingIds((prev) => prev.filter((id) => id !== userId))
                },
            },
        )
    }

    return (
        <Modal onClose={props.onClose}>
            <div className="flex items-center justify-between">
                <h2 className="text-lg font-semibold">
                    Invite to {props.groupName}
                </h2>
                <button
                    type="button"
                    onClick={props.onClose}
                    aria-label="Close"
                >
                    <X className="h-5 w-5" />
                </button>
            </div>

            <input
                type="text"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Search musicians by name..."
                className="w-full rounded-xl border border-foreground-low/30 px-3 py-2"
            />

            {inviteMember.isError && (
                <p className="text-sm text-red-500">
                    {getApiErrorMessage(inviteMember.error)}
                </p>
            )}

            <div className="flex max-h-72 flex-col gap-2 overflow-y-auto">
                {usersQuery.isPending && (
                    <p className="text-sm text-foreground-low">Loading...</p>
                )}

                {usersQuery.isError && (
                    <p className="text-sm text-red-500">
                        {getApiErrorMessage(usersQuery.error)}
                    </p>
                )}

                {!usersQuery.isPending &&
                    !usersQuery.isError &&
                    users.length === 0 && (
                        <p className="text-sm text-foreground-low">
                            No users found
                        </p>
                    )}

                {!usersQuery.isPending &&
                    !usersQuery.isError &&
                    users.map((user) => {
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
        </Modal>
    )
}
