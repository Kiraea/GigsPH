interface CheckTokenResponse {
    userId: string
} 
export const useAuth = () => {
    const user = useState<null|string>('auth-user', ()=> null);
    
    const setUser = (userId: string | null) => {
        user.value = userId;
    }
    const isLoading = useState<boolean>('auth-loading', ()=> false);
    
    const isLoggedIn = computed(()=> user.value !== null);
    
    const checkSessionToken = async () => {
        isLoading.value = true;
        try{
            const response = await $fetch<CheckTokenResponse>("/api/auth/check-token", {
               credentials:'include', 
            })
            setUser(response.userId);
        }catch(e:any){
            console.log(e);
            setUser(null);
        }finally {
            isLoading.value=false;
        }
    }
    
    
    return {user,setUser,checkSessionToken,isLoading,isLoggedIn}
}