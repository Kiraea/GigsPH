import { createFileRoute } from '@tanstack/react-router'
import {
    getMyBandInvitationQuery,
    type GetMyBandResponse,
    getMyBandsQuery,
} from '#/queries/bandQueries.tsx'
import { useQuery } from '@tanstack/react-query'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'
import { BandCard } from '#/components/BandCard.tsx'
import { useState } from 'react'
import { CreateBandModal } from '#/components/CreateBandModal.tsx'
import { UserModal } from '#/components/UserModal.tsx'
import { BandInvitationsCard } from '#/components/BandInvitationsCard.tsx'
import { TransferLeadershipModal } from '#/components/TransferLeadershipModal.tsx'
import { LeaveBandModal } from '#/components/LeaveBandModal.tsx'
import { CreatePostmodal } from '#/components/CreatePostmodal.tsx'

export const Route = createFileRoute('/_app/_authenticated/bands')({
    component: RouteComponent,
})

function RouteComponent() {
    const myBandsQuery = useQuery(getMyBandsQuery())

    const myBandInvitationsQuery = useQuery(getMyBandInvitationQuery())

    const invitations = myBandInvitationsQuery.data ?? []

    const bands = myBandsQuery.data ?? []

    const [isCreateModalOpen, setIsCreateModalOpen] = useState<boolean>(false)
    const [inviteBand, setInviteBand] = useState<GetMyBandResponse | null>(null)
    const [transferBand, setTransferBand] = useState<GetMyBandResponse | null>(
        null,
    )
    const [leaveBand, setLeaveBand] = useState<GetMyBandResponse | null>(null)

    const handleOnInvite = (band: GetMyBandResponse) => {
        setInviteBand(band)
    }

    const handleCreateModalOpen = () => {
        setIsCreateModalOpen(true)
    }

    const [postBand, setPostBand] = useState('')

    return (
        <div className="grid grid-cols-[minmax(0,1fr)_320px]">
            <div className=" flex flex-col border-foreground-high border-r-2 gap-5">
                <div className="flex justify-between">
                    <div className="flex-col">
                        <div className="text-4xl font-bold">Bands</div>
                        <div className="text-2xl font-medium text-foreground-medium">
                            Manage your bands, lineups, and posts
                        </div>
                    </div>

                    <button onClick={handleCreateModalOpen}>
                        {' '}
                        + Create Band
                    </button>
                </div>

                <div className="flex">
                    <h1>My Bands {bands.length}</h1>
                </div>

                <div className="flex flex-col">
                    {inviteBand && (
                        <UserModal
                            bandId={inviteBand.id}
                            bandName={inviteBand.name}
                            memberIds={inviteBand.members.map((m) => m.userId)}
                            onClose={() => setInviteBand(null)}
                        />
                    )}

                    {transferBand && (
                        <TransferLeadershipModal
                            band={transferBand}
                            onClose={() => setTransferBand(null)}
                        />
                    )}

                    {leaveBand && (
                        <LeaveBandModal
                            band={leaveBand}
                            onClose={() => setLeaveBand(null)}
                        />
                    )}

                    {postBand && (
                        <CreatePostmodal
                            onClose={() => setPostBand('')}
                            initialBandId={postBand}
                        />
                    )}

                    {isCreateModalOpen && (
                        <CreateBandModal
                            onClose={() => setIsCreateModalOpen(false)}
                        />
                    )}

                    {myBandsQuery.isPending && (
                        <div> Loading Your Bands...</div>
                    )}

                    {myBandsQuery.isError && (
                        <div> {getApiErrorMessage(myBandsQuery.error)} </div>
                    )}

                    {!myBandsQuery.isError &&
                        !myBandsQuery.isPending &&
                        bands.length === 0 && (
                            <div> Currently not in any band. </div>
                        )}

                    {bands.length > 0 && (
                        <div className="flex flex-col gap-5">
                            {bands.map((myBand) => (
                                <BandCard
                                    key={myBand.id}
                                    myBand={myBand}
                                    onInvite={handleOnInvite}
                                    onTransferLeadership={setTransferBand}
                                    onLeave={setLeaveBand}
                                    onPost={() => setPostBand(myBand.id)}
                                />
                            ))}
                        </div>
                    )}
                </div>
            </div>
            <div className="flex flex-col gap-4">
                <h2>Pending Invitations ({invitations.length})</h2>

                {myBandInvitationsQuery.isPending && (
                    <div>Loading invitations...</div>
                )}

                {myBandInvitationsQuery.isError && (
                    <div>
                        {getApiErrorMessage(myBandInvitationsQuery.error)}
                    </div>
                )}

                {!myBandInvitationsQuery.isError &&
                    !myBandInvitationsQuery.isPending &&
                    invitations.length === 0 && (
                        <div>No pending invitations.</div>
                    )}

                {invitations.length > 0 && (
                    <div className="flex flex-col gap-5">
                        {invitations.map((myInvitation) => (
                            <BandInvitationsCard
                                key={myInvitation.invitationId}
                                bandInvitation={myInvitation}
                            />
                        ))}
                    </div>
                )}
            </div>
        </div>
    )
}
