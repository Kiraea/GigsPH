import { useContext, useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useNavigate } from '@tanstack/react-router'
import { X } from 'lucide-react'
import { getUsersQuery, type PublicUserDto } from '#/queries/userQueries.tsx'
import { Modal } from '#/components/Modal.tsx'
import { useCreateGroupConversation } from '#/queries/conversationQueries.tsx'
import { AuthContext } from '#/contexts/authContext.tsx'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'

type CreateGroupModalProps = {
    onClose: () => void
}

export function CreateGroupModal(props: CreateGroupModalProps) {
    const [groupName, setGroupName] = useState('')
    const [search, setSearch] = useState('')
    const [debouncedSearch, setDebouncedSearch] = useState('')
    const [selectedUsers, setSelectedUsers] = useState<PublicUserDto[]>([])

    const usersQuery = useQuery(getUsersQuery(debouncedSearch))
    const auth = useContext(AuthContext)

    const users =
        usersQuery.data?.users.filter((user) => user.id !== auth?.user?.id) ??
        []

    const createGroup = useCreateGroupConversation()
    const navigate = useNavigate()

    const handleToggleUser = (user: PublicUserDto) => {
        const alreadySelected = selectedUsers.some((u) => u.id === user.id)
        if (alreadySelected) {
            setSelectedUsers((prev) => prev.filter((u) => u.id !== user.id))
        } else {
            setSelectedUsers((prev) => [...prev, user])
        }
    }

    const handleSubmit = () => {
        if (!groupName.trim() || selectedUsers.length === 0) {
            return
        }

        createGroup.mutate(
            {
                name: groupName.trim(),
                invitedUserIds: selectedUsers.map((u) => u.id),
            },
            {
                onSuccess: (data) => {
                    props.onClose()
                    navigate({
                        to: `/conversations/$conversationId`,
                        params: { conversationId: data.conversationId },
                    })
                },
            },
        )
    }

    useEffect(() => {
        const timeOut = setTimeout(() => {
            setDebouncedSearch(search)
        }, 400)

        return () => {
            clearTimeout(timeOut)
        }
    }, [search])

    return (
        <Modal onClose={props.onClose} closeDisabled={createGroup.isPending}>
            <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold">New Group Chat</h2>
                <button
                    type="button"
                    onClick={props.onClose}
                    disabled={createGroup.isPending}
                    aria-label="Close"
                >
                    <X className="h-5 w-5" />
                </button>
            </div>

            <input
                type="text"
                value={groupName}
                disabled={createGroup.isPending}
                onChange={(e) => setGroupName(e.target.value)}
                placeholder="Group name..."
                className="w-full rounded-xl border border-foreground-low/30 px-3 py-2"
            />

            {selectedUsers.length > 0 && (
                <div className="flex flex-wrap gap-2">
                    {selectedUsers.map((user) => (
                        <button
                            key={user.id}
                            type="button"
                            disabled={createGroup.isPending}
                            onClick={() => handleToggleUser(user)}
                            className="flex items-center gap-1 rounded-full bg-purple-400/30 px-3 py-1 text-sm"
                        >
                            <span>{user.displayName}</span>
                            <X className="h-3 w-3" />
                        </button>
                    ))}
                </div>
            )}

            <input
                type="text"
                value={search}
                disabled={createGroup.isPending}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Search musicians to invite..."
                className="w-full rounded-xl border border-foreground-low/30 px-3 py-2"
            />

            <div className="flex flex-col gap-1 max-h-48 overflow-y-auto">
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
                        const isSelected = selectedUsers.some(
                            (u) => u.id === user.id,
                        )
                        return (
                            <button
                                key={user.id}
                                type="button"
                                disabled={createGroup.isPending}
                                onClick={() => handleToggleUser(user)}
                                className={`rounded-xl px-3 py-2 text-left ${
                                    isSelected
                                        ? 'bg-background-low border border-purple-400'
                                        : 'bg-background-medium'
                                }`}
                            >
                                {user.displayName}
                            </button>
                        )
                    })}
            </div>

            {createGroup.isError && (
                <p className="text-sm text-red-500">
                    {getApiErrorMessage(createGroup.error)}
                </p>
            )}

            <button
                type="button"
                onClick={handleSubmit}
                disabled={
                    !groupName.trim() ||
                    selectedUsers.length === 0 ||
                    createGroup.isPending
                }
                className="rounded-xl bg-purple-400 px-4 py-2 text-foreground-high disabled:opacity-50"
            >
                {createGroup.isPending ? 'Creating...' : 'Create Group'}
            </button>
        </Modal>
    )
}
