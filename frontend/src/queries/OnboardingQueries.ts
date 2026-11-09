import { queryOptions } from '@tanstack/react-query'
import { axiosInstance } from '#/axiosInstance.tsx'
import { useQuery } from '@tanstack/react-query'

export type GenresOptions = {
    id: string
    name: string
}

export type InstrumentOptions = {
    id: string
    name: string
}
export const getGenresOptionsQuery = queryOptions({
    queryKey: ['genres'],
    queryFn: async () => {
        const response = await axiosInstance.get<GenresOptions[]>('/genres')
        return response.data
    },
})

export const getInstrumentOptionsQuery = queryOptions({
    queryKey: ['instruments'],
    queryFn: async () => {
        const response =
            await axiosInstance.get<InstrumentOptions[]>('/instruments')
        return response.data
    },
})
