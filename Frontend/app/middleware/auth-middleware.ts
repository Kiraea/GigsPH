import {useAuth} from "~/composables/useAuth";


export default defineNuxtRouteMiddleware(async (to,from)=> {
    const {isLoggedIn} = useAuth();
    
    if (!isLoggedIn.value){
        return navigateTo("/login")
    }
    
    
    
});