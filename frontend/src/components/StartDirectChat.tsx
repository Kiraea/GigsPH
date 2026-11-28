import { useContext, useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useNavigate } from '@tanstack/react-router'
import { X } from 'lucide-react'
import { getUsersQuery } from '#/queries/userQueries.tsx'
import { Modal } from '#/components/Modal.tsx'
import { useStartDirectConversation } from '#/queries/conversationQueries.tsx'
import { AuthContext } from '#/contexts/authContext.tsx'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'

type StartDirectChatProps = {
    onClose: () => void
}

export function StartDirectChat(props: StartDirectChatProps) {
    const [search, setSearch] = useState('')
    const [debouncedSearch, setDebouncedSearch] = useState('')

    const [selectedUserId, setSelectedUserId] = useState<string | null>(null)
    const [message, setMessage] = useState('')

    const usersQuery = useQuery(getUsersQuery(debouncedSearch))

    const auth = useContext(AuthContext)

    const users =
        usersQuery.data?.users.filter((user) => user.id !== auth?.user?.id) ??
        []

    const startDirectConversation = useStartDirectConversation()

    const navigate = useNavigate()

    const handleSubmit = () => {
        if (!selectedUserId) {
            return
        }

        if (!message.trim()) {
            return
        }

        startDirectConversation.mutate(
            {
                recipientUserId: selectedUserId,
                clientMessageId: crypto.randomUUID(),
                text: message.trim(),
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
        <Modal
            onClose={props.onClose}
            closeDisabled={startDirectConversation.isPending}
        >
            <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold">New Message</h2>
                <button
                    type="button"
                    onClick={props.onClose}
                    disabled={startDirectConversation.isPending}
                    aria-label="Close"
                >
                    <X className="h-5 w-5" />
                </button>
            </div>

            <input
                type="text"
                value={search}
                disabled={startDirectConversation.isPending}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Search musicians..."
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
                    users.map((user) => (
                        <button
                            type="button"
                            disabled={startDirectConversation.isPending}
                            className={`rounded-xl px-3 py-2 text-left ${
                                user.id === selectedUserId
                                    ? 'bg-background-low border border-purple-400'
                                    : 'bg-background-medium'
                            }`}
                            key={user.id}
                            onClick={() => setSelectedUserId(user.id)}
                        >
                            {user.displayName}
                        </button>
                    ))}
            </div>

            <textarea
                className="rounded-xl border border-foreground-low/30 p-3"
                value={message}
                disabled={startDirectConversation.isPending}
                onChange={(e) => setMessage(e.target.value)}
                placeholder="Write your first message..."
            />

            {startDirectConversation.isError && (
                <p className="text-sm text-red-500">
                    {getApiErrorMessage(startDirectConversation.error)}
                </p>
            )}

            <button
                type="button"
                onClick={handleSubmit}
                disabled={
                    !selectedUserId ||
                    !message.trim() ||
                    startDirectConversation.isPending
                }
                className="rounded-xl bg-purple-400 px-4 py-2 text-foreground-high disabled:opacity-50"
            >
                {startDirectConversation.isPending ? 'Sending...' : 'Send'}
            </button>
        </Modal>
    )
}
