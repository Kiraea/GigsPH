import { type PostResponse, useUpdateLike } from '#/queries/postQueries.ts'
import { useState } from 'react'
import { ConvertDateTimeToReadable } from '#/utils/generalUtils.ts'
import {
    ChevronLeft,
    ChevronRight,
    ExternalLink,
    Heart,
    MessageCircle,
} from 'lucide-react'

export function PostCard({ post }: { post: PostResponse }) {
    const [mediaIndex, setMediaIndex] = useState<number>(0)

    const [slideFrom, setSlideFrom] = useState<'left' | 'right'>('right')
    const media = post.media[mediaIndex]

    const updateLike = useUpdateLike()

    console.log(post)

    const handleLike = () => {
        updateLike.mutate({
            postId: post.id,
            request: { isLiked: !post.isLiked },
        })
    }

    return (
        <article className="flex flex-col rounded-2xl bg-background-medium shadow-sm">
            <div className="flex flex-col gap-5 p-5">
                <header className="flex gap-2 items-center">
                    <div>
                        <div className="rounded-full h-13 w-13 bg-purple-300 flex items-center justify-center">
                            {post.publisherName.slice(0, 1)}
                        </div>
                    </div>
                    <div className="flex flex-col">
                        <p className="font-semibold text-lg">
                            {post.publisherName}
                        </p>
                        <time className="font-light text-foreground-low text-sm">
                            {ConvertDateTimeToReadable(post.createdAt)}
                        </time>
                    </div>
                </header>

                <div className="flex flex-col gap-2 ">
                    <h2 className="font-bold text-3xl"> {post.title}</h2>
                    <p className="text-foreground-medium">
                        {' '}
                        {post.description}
                    </p>
                </div>
                {media && (
                    <div className="relative flex max-w-200 w-full gap-2 justify-between items-center self-center">
                        {post.media.length > 1 && (
                            <button
                                type="button"
                                className="absolute left-5 top-1/2"
                                disabled={mediaIndex === 0}
                                onClick={() => {
                                    setMediaIndex(mediaIndex - 1)
                                    setSlideFrom('left')
                                }}
                            >
                                <div className="rounded-full overflow-hidden p-2 bg-fixed-white/80">
                                    <ChevronLeft className="text-fixed-black"></ChevronLeft>
                                </div>
                            </button>
                        )}

                        <div className="aspect-video flex-1 bg-fixed-black rounded-2xl overflow-hidden">
                            <div
                                key={media.id}
                                className={`h-full w-full ${slideFrom == 'left' ? 'animate-[slide-from-right_180ms_ease-out] ' : 'animate-[slide-from-left_180ms_ease-out]'}`}
                            >
                                {media.contentType.startsWith('image/') && (
                                    <img
                                        src={media.url}
                                        alt={post.title}
                                        className="h-full w-full object-contain"
                                    />
                                )}

                                {media.contentType.startsWith('video/') && (
                                    <video
                                        src={media.url}
                                        controls
                                        className="h-full w-full object-contain"
                                    />
                                )}
                                {media.contentType.startsWith('audio/') && (
                                    <audio
                                        src={media.url}
                                        controls
                                        className="w-full h-full object-contain"
                                    />
                                )}
                            </div>
                        </div>

                        {post.media.length > 1 && (
                            <button
                                type="button"
                                className="absolute top-1/2 right-5"
                                disabled={mediaIndex === post.media.length - 1}
                                onClick={() => {
                                    setMediaIndex(mediaIndex + 1)
                                    setSlideFrom('right')
                                }}
                            >
                                <div className="rounded-full overflow-hidden p-2 bg-fixed-white/80">
                                    <ChevronRight className="text-fixed-black" />
                                </div>
                            </button>
                        )}
                    </div>
                )}

                {post.media.length > 1 && (
                    <div className="flex justify-center gap-2" role="status">
                        <span className="sr-only">
                            {' '}
                            {mediaIndex + 1} out of {post.media.length}{' '}
                        </span>
                        {post.media.map((attachment, index) => (
                            <span
                                key={attachment.id}
                                aria-hidden="true"
                                className={`h-1.5 rounded-full transition-full duration-300 ${
                                    index === mediaIndex
                                        ? 'w-5 bg-foreground-high'
                                        : 'w-1.5 bg-foreground-low/60'
                                }`}
                            ></span>
                        ))}
                    </div>
                )}

                {(post.genres.length > 0 || post.instruments.length > 0) && (
                    <div className="grid grid-cols-[auto_1fr] gap-x-4 gap-y-3 text-md">
                        {post.genres.length > 0 && (
                            <>
                                <p className="text-foreground-low text-sm">
                                    Genres
                                </p>
                                <ul className="flex flex-wrap gap-2">
                                    {post.genres.map((genre) => (
                                        <li
                                            className="rounded-2xl px-5 text-sm flex items-center justify-center bg-purple-200 text-purple-900"
                                            key={genre.id}
                                        >
                                            {genre.name}
                                        </li>
                                    ))}
                                </ul>
                            </>
                        )}

                        {post.instruments.length > 0 && (
                            <>
                                <p className="text-foreground-low text-sm">
                                    Instruments
                                </p>
                                <ul className="flex flex-wrap gap-2">
                                    {post.instruments.map((instrument) => (
                                        <li
                                            className="rounded-2xl px-5 text-sm flex items-center justify-center bg-background-low text-foreground-low"
                                            key={instrument.id}
                                        >
                                            {instrument.name}
                                        </li>
                                    ))}
                                </ul>
                            </>
                        )}
                    </div>
                )}
            </div>

            <footer className=" flex justify-between p-5">
                <div className="flex items-center-safe justify-center gap-5 ">
                    <button
                        type="button"
                        onClick={handleLike}
                        disabled={updateLike.isPending}
                    >
                        <Heart
                            className={
                                post.isLiked
                                    ? 'fill-purple-400 stroke-none'
                                    : ''
                            }
                            size={20}
                        />
                    </button>

                    <div>{post.likedCount} </div>
                    <button className="flex gap-2">
                        <MessageCircle className="text-foreground-low" />5
                    </button>
                </div>

                <ExternalLink className="text-foreground-low" />
            </footer>
        </article>
    )
}
