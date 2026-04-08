import {useAuth} from "~/composables/useAuth";
import {useUser} from "~/composables/useUser";

interface checkToken {
    userId:string
    isOnboarded:boolean,
    displayName: string,
}
export default defineNuxtPlugin(async () => {
    const { setUser } = useAuth()

    const { data, error } = await useFetch<checkToken>('/api/auth/check-token', {
        headers: useRequestHeaders(['cookie']) as HeadersInit,
    })
    if (data.value) {
        setUser(data.value.userId, data.value.isOnboarded);
    } else {
        setUser(null)
    }
})