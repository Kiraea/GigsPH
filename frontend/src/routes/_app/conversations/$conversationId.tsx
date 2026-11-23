import { createFileRoute } from '@tanstack/react-router'
import { useState } from 'react'
import { Ellipsis, PenLine, UserRoundPlus } from 'lucide-react'

export const Route = createFileRoute('/_app/conversations/$conversationId')({
    component: RouteComponent,
})

type GroupDetailResponse = {
    conversationId: number
    name: string
    userGroupChatDetails: UserGroupChatDetail[]
    isOwner: boolean
}

type UserGroupChatDetail = {
    userId: string
    displayName: string
}

function RouteComponent() {
    const [openConvoTab, setOpenConvoTab] = useState<boolean>(false)
    const dummyData = {
        conversationId: 106,
        conversationName: 'Weekend Recording',
        conversationType: 'group',
        directRequestStatus: null,
        canAcceptRequest: false,
        latestMessage: null,
        latestSender: null,
        latestDate: null,
    }
    const dummyGroupDetails: GroupDetailResponse = {
        conversationId: dummyData.conversationId,
        name: dummyData.conversationName,
        userGroupChatDetails: [
            { userId: 'user-alex', displayName: 'Alex Cruz' },
            { userId: 'user-sam', displayName: 'Sam Rivera' },
            { userId: 'user-maya', displayName: 'Maya Santos' },
            { userId: 'user-ari', displayName: 'Ari Mendoza' },
        ],
        isOwner: true,
    }

    return (
        <div className="h-full flex">
            <div className="flex-1 flex flex-col gap-5">
                <div className="flex justify-between border-b border-foreground-low">
                    <div className="flex flex-col p-5">
                        <label className="font-bold text-2xl">
                            {dummyData.conversationName}
                        </label>
                        <div className="flex">
                            <p>
                                {dummyGroupDetails.userGroupChatDetails.length}{' '}
                                Members
                            </p>
                            <p>*</p>
                            <p className="first-letter:uppercase">
                                {dummyData.conversationType} Chat
                            </p>
                        </div>
                    </div>
                    <div className="flex">
                        <button
                            className="p-2"
                            onClick={() => setOpenConvoTab(!openConvoTab)}
                        >
                            Toggle
                        </button>
                    </div>
                </div>

                <div className="overflow-y-auto">
                    adsa dsadsa ddsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa dsadsa
                    dsadsa dsadsasadsa dsadsa dsadsa dsadsa dsadsa
                </div>
                <div className="flex"></div>
            </div>

            {openConvoTab && (
                <div className="shrink-0 w-64 border-l-1 border-foreground-low flex flex-col justify-between ">
                    <div className="flex flex-col">
                        <div className="grid grid-cols-[1fr_auto]  p-5 gap-y-4">
                            <div className="col-span-2 grid grid-cols-subgrid w-full">
                                <p className="font-bold text-2xl">Title</p>
                                <PenLine />
                            </div>
                            <div className="col-span-2 grid grid-cols-subgrid w-ful">
                                <p className="">Members (5) </p>
                                <UserRoundPlus />
                            </div>
                        </div>
                        <div className="flex flex-col p-5">
                            {dummyGroupDetails.userGroupChatDetails.length >
                                0 &&
                                dummyGroupDetails.userGroupChatDetails.map(
                                    (detail) => {
                                        return (
                                            <div className="flex items-center justify-between ">
                                                <label>
                                                    {detail.displayName}
                                                </label>
                                                <Ellipsis />
                                            </div>
                                        )
                                    },
                                )}
                        </div>
                    </div>
                    <button className=" border-t-1 border-foreground-low">
                        Leave Group
                    </button>
                </div>
            )}
        </div>
    )
}
