import { axiosInstance } from '#/axiosInstance.tsx'
import type {
    GenresOptions,
    InstrumentOptions,
} from '#/queries/OnboardingQueries.ts'
import {
    type InfiniteData,
    infiniteQueryOptions,
    useMutation,
    useQueryClient,
} from '@tanstack/react-query'

export type PostResponse = {
    id: string
    title: string
    description: string | null
    bandId: string | null
    userId: string | null
    publisherName: string
    likedCount: number
    isLiked: boolean
    media: MediaResponse[]
    genres: GenresOptions[]
    instruments: InstrumentOptions[]
    createdAt: string
}

export type MediaResponse = {
    id: string
    contentType: string
    url: string
}

export type GetPublicPostsResponse = {
    posts: PostResponse[]
    cursor: string | null
}

export type CreatePostRequest = {
    title: string
    description: string
    bandId?: string
    genreIds?: string[]
    instrumentIds?: string[]
    uploadIds: string[]
}

export type CreatePostResponse = {
    postId: string
}

export type SetLikeRequest = {
    isLiked: boolean
}

export type SetLikeResponse = {
    isLiked: boolean
    postId: string
}

const createPost = async (request: CreatePostRequest) => {
    const response = await axiosInstance.post<CreatePostResponse>(
        '/posts',
        request,
    )
    return response.data
}

const updateLike = async ({
    request,
    postId,
}: {
    request: SetLikeRequest
    postId: string
}): Promise<SetLikeResponse> => {
    const response = await axiosInstance.put<SetLikeResponse>(
        `/posts/${postId}/like`,
        request,
    )
    return response.data
}
export const useUpdateLike = () => {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: updateLike,
        onSuccess: (data) => {
            // hover in data in useinfinitequery
            queryClient.setQueriesData<InfiniteData<GetPublicPostsResponse>>(
                { queryKey: ['posts'] },
                (oldData) => {
                    if (!oldData) return oldData

                    return {
                        ...oldData,
                        pages: oldData.pages.map((page) => ({
                            ...page,
                            posts: page.posts.map((post) =>
                                post.id === data.postId
                                    ? {
                                          ...post,
                                          isLiked: data.isLiked,
                                          /*
                                          likedCount: data.isLiked
                                              ? post.likedCount + 1
                                              : post.likedCount - 1,
                                             
                                     */
                                          likedCount:
                                              data.isLiked === post.isLiked
                                                  ? post.likedCount
                                                  : post.likedCount +
                                                    (data.isLiked ? 1 : -1),
                                      }
                                    : post,
                            ),
                        })),
                    }
                },
            )
        },
    })
}

export const useCreatePost = () => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: createPost,
        onSuccess: async (data) => {
            await queryClient.invalidateQueries({ queryKey: ['posts'] })
        },
    })
}

export const getPublicPostQuery = (viewerId?: string) => {
    return infiniteQueryOptions({
        queryKey: ['posts', viewerId || 'anonymous'],
        initialPageParam: null as string | null,
        queryFn: async ({ pageParam }) => {
            const response = await axiosInstance.get<GetPublicPostsResponse>(
                '/posts',
                {
                    params: pageParam ? { cursor: pageParam } : undefined,
                },
            )
            return response.data
        },
        getNextPageParam: (lastPage) => lastPage.cursor,
    })
}
