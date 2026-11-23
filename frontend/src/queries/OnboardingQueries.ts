import { queryOptions, useMutation } from '@tanstack/react-query'
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

export type OnboardingRequest = {
    firstName: string
    lastName: string
    displayName: string
    description: string | null
    genreIds: string[]
    instruments: {
        instrumentId: string
        isMain: boolean
    }[]
    socialLinks: {
        platform: 'Facebook' | 'Twitter'
        url: string
    }[]
    location: {
        country: string
        provinceState: string
        city: string
    }
}
export type OnboardingResponse = {
    id: string
    displayName: string | null
    isOnboarded: boolean
}

async function SubmitOnboarding(
    request: OnboardingRequest,
): Promise<OnboardingResponse> {
    const response = await axiosInstance.post<OnboardingResponse>(
        '/user/onboarding',
        request,
    )

    return response.data
}

export function useOnboarding() {
    return useMutation({
        mutationFn: SubmitOnboarding,
    })
}
