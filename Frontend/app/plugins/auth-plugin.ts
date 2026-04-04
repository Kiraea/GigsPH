import {useAuth} from "~/composables/useAuth";
interface checkToken {
    userId:string
}
export default defineNuxtPlugin(async () => {
    const { setUser } = useAuth()

    const { data, error } = await useFetch<{ userId: string }>('/api/auth/check-token', {
        headers: useRequestHeaders(['cookie']) as HeadersInit,
    })

    console.log('check-token data:', data.value)
    console.log('check-token error:', error.value)

    if (data.value) {
        setUser(data.value.userId)
    } else {
        setUser(null)
    }
})