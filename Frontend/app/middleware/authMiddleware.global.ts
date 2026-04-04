import {useAuth} from "~/composables/useAuth";
import {useUser} from "~/composables/useUser";

export default defineNuxtRouteMiddleware(async (to,from)=> {
    const {isLoggedIn} = useAuth();
    const {profile} = useUser();
    console.log("hit hit hit hit")
    const publicRoutes: string[] = ["/", "/landing"];
    
    const isPublicRoute = publicRoutes.includes(to.path)
    
    const authRoutes: string[] = ["/login", "/register"];

    const isAuthRoutes = authRoutes.includes(to.path);
    

    if (!isLoggedIn.value){
        if (!isPublicRoute && !isAuthRoutes){
            return navigateTo("/login");
        }else{
            return
        }
    }
    if (isLoggedIn.value){
        
        if (isAuthRoutes){
            return navigateTo("/")
        }
        
        if (profile.value?.isOnboarded){
            if (to.path === "/onboarding"){
                return navigateTo("/")
            }
            return
        }else{
            if(to.path !== "/onboarding"){
                return navigateTo("/onboarding");
            }
            return
        }
        
    }
    
    

    
    
    
});