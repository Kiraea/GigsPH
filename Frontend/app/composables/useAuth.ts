interface CheckTokenResponse {
    userId: string,
    isOnboarded: boolean
} 
export const useAuth = () => {
    const user = useState<{id: string, isOnboarded: boolean} | null>('auth-user', ()=> null);
    const isLoading = useState<boolean>('auth-loading', ()=> false);
    const setUser = (userId: string | null, isOnboarded = false) => {
        if (userId){
            user.value = {id:userId, isOnboarded:isOnboarded}
        }else{
            user.value =null
        }
    }
    
    const isLoggedIn = computed(()=> user.value !== null);
    

    
    return {user,setUser,isLoading,isLoggedIn}
}