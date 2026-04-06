import {useAuth} from "~/composables/useAuth";
import {useUser} from "~/composables/useUser";

interface checkToken {
    userId:string
    isOnboarded:boolean,
    displayName: string,
}
export default defineNuxtPlugin(async () => {
    const { setUser } = useAuth()
    const {setProfile }= useUser()

    const { data, error } = await useFetch<checkToken>('/api/auth/check-token', {
        headers: useRequestHeaders(['cookie']) as HeadersInit,
    })

    if (data.value) {
        setUser(data.value.userId)
        setProfile({
            isOnboarded:data.value.isOnboarded,
            displayName:data.value.displayName
        })
        
    } else {
        
    }
})