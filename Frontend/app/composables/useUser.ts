import {useErrorHandler} from "~/composables/useErrorHandler";


export interface UserProfile {
    displayName: string,
    isOnboarded: boolean
}


export const useUser = () => {
    const profile = useState<UserProfile | null>('user-profile', () => null);
    const {setErrors, errors} = useErrorHandler();
    
    const isMutating = ref(false) ;
    const setProfile = (data: UserProfile | null) => {
        profile.value = data
    }
    
    const onboard = async (payload: OnboardPayload) => {
        isMutating.value = true
        try{
            let response = await userUtil.onboard(payload)
            setProfile({
                displayName: response.displayName,
                isOnboarded: true,
            })
        }catch(e){
            setErrors(e);
        }
        finally{
            isMutating.value = false;
        }

        console.log(profile.value);
    }
    return {profile,setProfile, onboard}
}