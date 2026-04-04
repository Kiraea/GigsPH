interface CheckTokenResponse {
    userId: string
} 
export const useAuth = () => {
    const user = useState<null|string>('auth-user', ()=> null);
    const isLoading = useState<boolean>('auth-loading', ()=> false);
    const setUser = (userId: string | null) => {
        user.value = userId;
    }
    
    const isLoggedIn = computed(()=> user.value !== null);
    
    
    console.log(user.value, isLoggedIn.value);

    
    return {user,setUser,isLoading,isLoggedIn}
}