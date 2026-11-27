import { axiosInstance } from '#/axiosInstance.tsx'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import axios from 'axios'
export type RegisterRequest = {
    email: string
    password: string
}

export type RegisterResponse = {
    email: string
}

async function register(request: RegisterRequest): Promise<RegisterResponse> {
    try {
        const response = await axiosInstance.post<RegisterResponse>(
            '/auth/register',
            request,
        )
        return response.data
    } catch (e) {
        if (axios.isAxiosError(e)) {
            console.log(e.message)
        }
        throw e
    }
}
export function useRegister() {
    return useMutation({
        mutationFn: register,
    })
}

async function login(request: LoginRequest): Promise<LoginResponse> {
    try {
        const response = await axiosInstance.post<LoginResponse>(
            'auth/login',
            request,
        )
        return response.data
    } catch (e) {
        if (axios.isAxiosError(e)) {
            console.log(e.message)
        }
        throw e
    }
}

export function useLogin() {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: login,
        onSuccess: () => {
            queryClient.clear()
        },
    })
}

export type LoginRequest = {
    usernameOrEmail: string
    password: string
}

export type LoginResponse = {
    userName: string
    email: string
    accessToken: string
}
