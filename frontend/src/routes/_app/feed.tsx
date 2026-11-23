import { createFileRoute } from '@tanstack/react-router'
export const Route = createFileRoute('/_app/feed')({
    component: FeedPage,
})

import type {
    GenresOptions,
    InstrumentOptions,
} from '#/queries/OnboardingQueries.ts'
import { PostCard } from '#/components/PostCard.tsx'

export type PostResponse = {
    id: string
    title: string
    description: string | null
    bandId: number | null
    userId: number
    publisherName: string
    likedCount: number
    isLiked: boolean
    media: MediaResponse[]
    Genres: GenresOptions[]
    Instruments: InstrumentOptions[]
    CreatedAt: string
}

export type MediaResponse = {
    id: number
    contentType: string
    Url: string
}

function FeedPage() {
    const dummyPostData: PostResponse[] = [
        {
            id: 'post-1',
            CreatedAt: '2026-11-22T03:00:00Z',
            title: 'Sunday jazz jam',
            description:
                'We opened with a slow blues and switched to a faster groove halfway through. The recording starts just before the drum solo.',
            bandId: null,
            userId: 1,
            publisherName: 'Sam Rivera',
            likedCount: 42,
            isLiked: true,
            media: [
                {
                    id: 1,
                    contentType: 'video/mp4',
                    Url: '/media/test.mp4',
                },
                {
                    id: 2,
                    contentType: 'image/jpeg',
                    Url: '/media/test.jpg',
                },
            ],
            Genres: [
                { id: 1, name: 'Jazz' },
                { id: 2, name: 'Blues' },
            ],
            Instruments: [
                { id: 1, name: 'Drums' },
                { id: 2, name: 'Piano' },
                { id: 3, name: 'Upright Bass' },
            ],
        },
        {
            id: 'post-2',
            CreatedAt: '2026-11-21T03:00:00Z',
            title: 'First demo of Paper Lanterns',
            description:
                'I recorded this at home with one mic between the guitar and my voice. I’m deciding whether to keep the second verse quiet or build into the chorus.',
            bandId: null,
            userId: 2,
            publisherName: 'Maya Santos',
            likedCount: 17,
            isLiked: false,
            media: [
                {
                    id: 3,
                    contentType: 'audio/mpeg',
                    Url: '/media/test.mp3',
                },
            ],
            Genres: [{ id: 3, name: 'Indie Pop' }],
            Instruments: [
                { id: 4, name: 'Acoustic Guitar' },
                { id: 5, name: 'Vocals' },
            ],
        },
        {
            id: 'post-3',
            CreatedAt: '2026-11-20T03:00:00Z',
            title: 'Friday rooftop set',
            description:
                'We tried a slower ending for Corner Store and gave the bass a few extra bars. The second photo has our set list with the changes we made during soundcheck.',
            bandId: null,
            userId: 3,
            publisherName: 'Nico Reyes',
            likedCount: 128,
            isLiked: true,
            media: [
                {
                    id: 4,
                    contentType: 'image/jpeg',
                    Url: '/media/test.jpg',
                },
                {
                    id: 5,
                    contentType: 'image/jpeg',
                    Url: '/media/test.jpg',
                },
                {
                    id: 6,
                    contentType: 'video/mp4',
                    Url: '/media/test.mp4',
                },
            ],
            Genres: [
                { id: 4, name: 'Indie Rock' },
                { id: 5, name: 'Dream Pop' },
            ],
            Instruments: [
                { id: 1, name: 'Drums' },
                { id: 5, name: 'Vocals' },
                { id: 6, name: 'Electric Guitar' },
                { id: 7, name: 'Bass Guitar' },
            ],
        },
        {
            id: 'post-4',
            CreatedAt: '2026-11-19T03:00:00Z',
            title: 'Quiet piano take at midnight',
            description: null,
            bandId: null,
            userId: 4,
            publisherName: 'Lea Cruz',
            likedCount: 8,
            isLiked: false,
            media: [
                {
                    id: 7,
                    contentType: 'audio/mpeg',
                    Url: '/media/test.mp3',
                },
            ],
            Genres: [{ id: 6, name: 'Ambient' }],
            Instruments: [{ id: 2, name: 'Piano' }],
        },
        {
            id: 'post-5',
            CreatedAt: '2026-11-18T03:00:00Z',
            title: 'A bass line looking for a drum groove',
            description:
                'I’ve got eight bars at 94 BPM in E minor. I’d like to try a sparse drum groove with space for the bass fills. I’ll record it after Sunday practice.',
            bandId: null,
            userId: 5,
            publisherName: 'Ari Mendoza',
            likedCount: 0,
            isLiked: false,
            media: [],
            Genres: [{ id: 7, name: 'Funk' }],
            Instruments: [{ id: 7, name: 'Bass Guitar' }],
        },
        {
            id: 'post-6',
            CreatedAt: '2026-11-17T03:00:00Z',
            title: 'Night Bus synth sketch',
            description:
                'A delayed synth answers the bass every other bar. I included a short loop and a photo of the patch settings for anyone who wants to try it.',
            bandId: null,
            userId: 4,
            publisherName: 'Lea Cruz',
            likedCount: 29,
            isLiked: true,
            media: [
                {
                    id: 8,
                    contentType: 'audio/mpeg',
                    Url: '/media/test.mp3',
                },
                {
                    id: 9,
                    contentType: 'image/jpeg',
                    Url: '/media/test.jpg',
                },
            ],
            Genres: [
                { id: 6, name: 'Ambient' },
                { id: 8, name: 'Synthwave' },
            ],
            Instruments: [{ id: 8, name: 'Keyboard / Synth' }],
        },
        {
            id: 'post-7',
            CreatedAt: '2026-11-16T03:00:00Z',
            title: 'Notes from Thursday rehearsal',
            description:
                'The full set runs 35 minutes. We’ll practice the transition into the last song again next week and see if it works without a count-in.',
            bandId: null,
            userId: 1,
            publisherName: 'Sam Rivera',
            likedCount: 4,
            isLiked: false,
            media: [],
            Genres: [],
            Instruments: [],
        },
        {
            id: 'post-8',
            CreatedAt: '2026-11-15T03:00:00Z',
            title: 'Three harmonies for the chorus',
            description:
                'The first take has a high harmony, the second doubles the melody, and the last adds a low part. The low harmony leaves the most room for the guitar between lines.',
            bandId: null,
            userId: 2,
            publisherName: 'Maya Santos',
            likedCount: 11,
            isLiked: false,
            media: [
                {
                    id: 10,
                    contentType: 'video/mp4',
                    Url: '/media/test.mp4',
                },
            ],
            Genres: [{ id: 3, name: 'Indie Pop' }],
            Instruments: [
                { id: 4, name: 'Acoustic Guitar' },
                { id: 5, name: 'Vocals' },
                { id: 9, name: 'Backing Vocals' },
            ],
        },
    ]

    return (
        <div className="min-h-screen grid grid-cols-3 bg-background-high">
            <div className="col-span-2 col-start-1 flex items-center justify-center">
                <div className="flex flex-col max-w-180 items-center justify-center gap-5">
                    <div className="flex p-5 shadow rounded-2xl w-full items-center ">
                        <div className="flex w-1/10 ">
                            <div className="h-13 w-13 rounded-full bg-background-low p-5 flex items-center justify-center">
                                A
                            </div>
                        </div>
                        <button className="flex-1 bg-background-medium rounded-2xl p-2 text-start text-foreground-low">
                            Share a clip!
                        </button>
                    </div>
                    {dummyPostData.map((post) => {
                        return <PostCard key={post.id} post={post} />
                    })}
                </div>
            </div>
            <div className="col-start-3 bg-background-high"></div>
        </div>
    )
}
