import { axiosInstance } from '#/axiosInstance.tsx'
import { queryOptions, useInfiniteQuery } from '@tanstack/react-query'

export type GetUsersResponse = {
    users: PublicUserDto[]
    page: number
    pageSize: number
    totalCount: number
    hasMore: boolean
}

export type PublicUserDto = {
    id: string
    displayName: string
    instruments: PublicUserInstrumentsDto[]
    genres: PublicUserGenresDto[]
}

export type PublicUserInstrumentsDto = {
    id: string
    name: string
    isMain: boolean
}

export type PublicUserGenresDto = {
    id: string
    name: string
}

const getUsers = async ({
    search,
    page,
    pageSize,
}: {
    search?: string
    page: number
    pageSize: number
}): Promise<GetUsersResponse> => {
    const response = await axiosInstance.get<GetUsersResponse>(`/users`, {
        params: {
            search: search?.trim() || null,
            pageSize: pageSize,
            page: page,
        },
    })
    return response.data
}

export const getUsersQuery = (
    search?: string,
    page: number = 1,
    pageSize: number = 10,
) => {
    return queryOptions({
        queryKey: ['users', search, page, pageSize],
        queryFn: () => getUsers({ search, page, pageSize }),
    })
}
