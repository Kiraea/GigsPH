import { createFileRoute, useNavigate } from '@tanstack/react-router'
export const Route = createFileRoute('/_app/feed')({
    component: FeedPage,
})

import { PostCard } from '#/components/PostCard.tsx'
import { useContext, useState } from 'react'
import { AuthContext } from '#/contexts/authContext.tsx'
import { useInfiniteQuery } from '@tanstack/react-query'
import { getPublicPostQuery } from '#/queries/postQueries.ts'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'
import { CreatePostmodal } from '#/components/CreatePostmodal.tsx'

function FeedPage() {
    const auth = useContext(AuthContext)
    const {
        data,
        isPending,
        isError,
        error,
        fetchNextPage,
        hasNextPage,
        isFetchingNextPage,
    } = useInfiniteQuery(getPublicPostQuery(auth?.user?.id))

    const [isCreateModalopen, setIsCreateModalopen] = useState(false)

    const posts = data?.pages.flatMap((page) => page.posts ?? []) ?? []
    const navigate = useNavigate()

    const handleShareClick = () => {
        if (!auth?.isLoggedIn) {
            navigate({ to: '/login' })
            return
        }
        setIsCreateModalopen(true)
    }

    return (
        <div className="min-h-screen grid grid-cols-3 bg-background-high">
            <div className="col-span-2 col-start-1 flex items-start justify-center">
                <div className="flex flex-col w-full max-w-180 justify-center gap-5">
                    <div className="flex p-5 max-w-180 shadow rounded-2xl w-full items-center ">
                        <div className="flex w-1/10 ">
                            <div className="h-13 w-13 rounded-full bg-background-low p-5 flex items-center justify-center">
                                A
                            </div>
                        </div>
                        <button
                            className="flex-1 bg-background-medium rounded-2xl p-2 text-start text-foreground-low"
                            onClick={handleShareClick}
                        >
                            Share a clip!
                        </button>
                        {isCreateModalopen && (
                            <CreatePostmodal
                                onClose={() => setIsCreateModalopen(false)}
                            />
                        )}
                    </div>

                    {isPending && <div>Loading...</div>}

                    {isError && <div>{getApiErrorMessage(error)}</div>}

                    {!isPending && !isError && posts.length == 0 && (
                        <div>No Posts Available</div>
                    )}

                    {posts.map((post) => {
                        return <PostCard key={post.id} post={post} />
                    })}

                    {hasNextPage && (
                        <button
                            className=""
                            disabled={isFetchingNextPage}
                            onClick={() => fetchNextPage()}
                        >
                            {isFetchingNextPage
                                ? 'Fetching Next Page...'
                                : 'Next'}
                        </button>
                    )}
                </div>
            </div>
            <div className="col-start-3 bg-background-high"></div>
        </div>
    )
}
