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
    

    
    return {user,setUser,isLoading,isLoggedIn}
}