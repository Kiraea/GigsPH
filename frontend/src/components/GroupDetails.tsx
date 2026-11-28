import { useContext, useState } from 'react'
import {
    Crown,
    Ellipsis,
    PenLine,
    UserRoundPlus,
    UserRoundX,
} from 'lucide-react'
import {
    type GetGroupDetailsResponse,
    useLeaveGroup,
    useRemoveGroupMember,
    useTransferGroupOwnership,
    useUpdateGroup,
} from '#/queries/conversationQueries.tsx'
import { InviteGroupMemberModal } from '#/components/InviteGroupMemberModal.tsx'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'
import { AuthContext } from '#/contexts/authContext.tsx'
import { useNavigate } from '@tanstack/react-router'

type GroupDetailsProps = {
    groupDetails: GetGroupDetailsResponse
}

export function GroupDetails(props: GroupDetailsProps) {
    const [isInviteModalOpen, setIsInviteModalOpen] = useState(false)
    const members = props.groupDetails.userGroupChatDetails

    // rename // remove mmeber/ transfer ownership // leavegroup

    const [isMenuOpen, setIsMenuOpen] = useState<string | null>(null)

    const removeGroupMember = useRemoveGroupMember()
    const transferOwnership = useTransferGroupOwnership()

    const leaveGroup = useLeaveGroup()

    const [isEditingName, setIsEditingName] = useState(false)
    const [name, setName] = useState('')

    const updateGroup = useUpdateGroup()

    const actionError =
        removeGroupMember.error ?? transferOwnership.error ?? updateGroup.error

    const handleUpdateGroup = () => {
        if (!name.trim()) {
            return
        }
        updateGroup.mutate(
            {
                conversationId: props.groupDetails.conversationId,
                request: { name: name.trim() },
            },
            {
                onSuccess: () => {
                    setIsEditingName(false)
                },
            },
        )
    }

    const auth = useContext(AuthContext)
    const handleRemoveGroupMember = () => {
        if (!isMenuOpen) {
            return
        }
        removeGroupMember.mutate(
            {
                conversationId: props.groupDetails.conversationId,
                userId: isMenuOpen,
            },
            {
                onSuccess: () => {
                    setIsMenuOpen(null)
                },
            },
        )
    }

    const handleTransferOwnership = () => {
        if (!isMenuOpen) {
            return
        }
        transferOwnership.mutate(
            {
                conversationId: props.groupDetails.conversationId,
                request: { newOwnerUserId: isMenuOpen },
            },
            {
                onSuccess: () => {
                    setIsMenuOpen(null)
                },
            },
        )
    }

    const navigate = useNavigate()
    const handleLeaveGroup = () => {
        leaveGroup.mutate(props.groupDetails.conversationId, {
            onSuccess: () => {
                navigate({ to: '/conversations' })
            },
        })
    }

    return (
        <div className="shrink-0 w-64 border-l-1 border-foreground-low flex flex-col justify-between ">
            <div className="flex flex-col">
                <div className="grid grid-cols-[minmax(0,1fr)_auto]  p-5 gap-y-4">
                    {isEditingName ? (
                        <div className="col-span-2 flex flex-col gap-2">
                            <input
                                type="text"
                                value={name}
                                onChange={(e) => setName(e.target.value)}
                                className="w-full"
                            />
                            <div className="flex gap-2">
                                <button
                                    type="button"
                                    disabled={
                                        updateGroup.isPending || !name.trim()
                                    }
                                    onClick={handleUpdateGroup}
                                >
                                    {updateGroup.isPending
                                        ? 'Saving...'
                                        : 'Save'}
                                </button>
                                <button
                                    type="button"
                                    disabled={updateGroup.isPending}
                                    onClick={() => setIsEditingName(false)}
                                >
                                    Cancel
                                </button>
                            </div>
                        </div>
                    ) : (
                        <div className="col-span-2 grid grid-cols-subgrid w-full">
                            <p className="font-bold text-2xl">
                                {props.groupDetails.name}
                            </p>

                            {props.groupDetails.isOwner && (
                                <button
                                    type="button"
                                    aria-label="Rename group"
                                    onClick={() => {
                                        setName(props.groupDetails.name)
                                        setIsEditingName(true)
                                    }}
                                >
                                    <PenLine />
                                </button>
                            )}
                        </div>
                    )}
                    <div className="col-span-2 grid grid-cols-subgrid w-full">
                        <p className="">Members : {members.length} </p>
                        {props.groupDetails.isOwner && (
                            <button
                                type="button"
                                onClick={() => setIsInviteModalOpen(true)}
                                aria-label="Invite member"
                            >
                                <UserRoundPlus />
                            </button>
                        )}
                    </div>
                </div>
                {actionError && (
                    <p className="px-5 text-sm text-red-500">
                        {getApiErrorMessage(actionError)}
                    </p>
                )}
                <div className="flex flex-col p-5">
                    {members.map((detail) => {
                        return (
                            <div
                                key={detail.userId}
                                className="flex items-center justify-between relative "
                            >
                                <label>{detail.displayName}</label>
                                {props.groupDetails.isOwner &&
                                    detail.userId != auth?.user?.id && (
                                        <button
                                            type="button"
                                            onClick={() =>
                                                setIsMenuOpen(detail.userId)
                                            }
                                        >
                                            <Ellipsis />
                                        </button>
                                    )}
                                {isMenuOpen === detail.userId && (
                                    <>
                                        <div
                                            className="fixed inset-0 z-10"
                                            onClick={() => setIsMenuOpen(null)}
                                        ></div>

                                        <div className=" absolute bottom-full right-0 z-20 rounded-2xl text-sm  bg-background-low grid grid-cols-[auto_1fr] ">
                                            <button
                                                type="button"
                                                className="p-2 grid col-span-2 z-20 grid-cols-subgrid"
                                                onClick={
                                                    handleRemoveGroupMember
                                                }
                                            >
                                                <UserRoundX />
                                                <p className="">
                                                    Remove Group Member
                                                </p>
                                            </button>
                                            <button
                                                type="button"
                                                className="p-2 grid col-span-2 grid-cols-subgrid"
                                                onClick={
                                                    handleTransferOwnership
                                                }
                                            >
                                                <Crown />
                                                <p className="">
                                                    Transfer Ownership
                                                </p>
                                            </button>
                                        </div>
                                    </>
                                )}
                            </div>
                        )
                    })}
                </div>
            </div>

            {leaveGroup.isError && (
                <div>{getApiErrorMessage(leaveGroup.error)}</div>
            )}
            <button
                disabled={leaveGroup.isPending}
                onClick={handleLeaveGroup}
                className=" border-t-1 border-foreground-low"
            >
                {leaveGroup.isPending ? 'Leaving group...' : 'Leave Group'}
            </button>

            {isInviteModalOpen && (
                <InviteGroupMemberModal
                    conversationId={props.groupDetails.conversationId}
                    groupName={props.groupDetails.name}
                    memberIds={members.map((m) => m.userId)}
                    onClose={() => setIsInviteModalOpen(false)}
                />
            )}
        </div>
    )
}
