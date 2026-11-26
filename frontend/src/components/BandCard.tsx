import type {
    BandMember,
    BandMemberInstrument,
    GetMyBandResponse,
} from '#/queries/bandQueries.tsx'
import { Crown } from 'lucide-react'
export function BandCard({
    myBand,
    onInvite,
}: {
    myBand: GetMyBandResponse
    onInvite: (band: GetMyBandResponse) => void
}) {
    const formatInstrument = (instruments: BandMemberInstrument[]) => {
        if (instruments.length == 0) return ''

        // find the main instruments
        const primary = instruments.find((ins) => ins.isMain) ?? instruments[0]

        if (instruments.length == 1) return primary.name

        return `${primary.name} + ${instruments.length - 1}`
    }

    const isLeader = myBand.myRole === 'Leader'

    return (
        <div className="flex flex-col text-foreground-high bg-background-medium p-5 gap-5 rounded-2xl ">
            <div className="flex justify-between ">
                <div className="flex gap-2 items-center">
                    <div className="w-12 h-12 rounded-full bg-purple-300 flex items-center justify-center text-foreground-high">
                        {myBand.name.substring(0, 1)}
                    </div>
                    <div className="flex flex-col">
                        <h1 className="text-4xl font-semibold">
                            {myBand.name}
                        </h1>
                        {myBand.description && (
                            <p className="text-foreground-low">
                                {myBand.description}
                            </p>
                        )}
                    </div>
                </div>

                <p>{myBand.memberCount} Members</p>
            </div>
            <hr className="border-t border-foreground-low" />

            <div className="flex flex-col gap-5">
                <label className="text-2xl font-bold">LineUp</label>

                <div className="flex gap-2">
                    {myBand.members.map((member, index) => (
                        <div key={member.userId}>
                            <div className="flex items-center gap-3">
                                <div className="relative h-10 w-10 rounded-full bg-purple-300 flex items-center justify-center">
                                    {member.displayName.substring(0, 1)}
                                    <div className="absolute bottom-1/2 left-6 ">
                                        <Crown className="fill-pink-400" />
                                    </div>
                                </div>
                                <div className="flex flex-col">
                                    <p className="font-semibold text-lg">
                                        {member.displayName}
                                    </p>

                                    <div className="relative group ">
                                        <p
                                            className="bg-background-low text-foreground-high rounded-2xl p-1"
                                            title={member.instruments
                                                .map((ins) => ins.name)
                                                .join(', ')}
                                        >
                                            {formatInstrument(
                                                member.instruments,
                                            )}
                                        </p>
                                        <div className="absolute bottom-full left-full w-max bg-background-medium text-foreground-medium flex-col p-2 gap-5 hidden group-hover:block">
                                            {member.instruments.map((ins) => (
                                                <p>{ins.name}</p>
                                            ))}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            </div>

            <hr className="border-t border-foreground-low" />

            <div className="flex justify-between">
                <button type="button">Post As Band</button>

                <button
                    type="button"
                    disabled={!isLeader}
                    onClick={() => onInvite(myBand)}
                >
                    Invite User
                </button>

                <button>Leave</button>
            </div>
        </div>
    )
}
