import {useAuth} from "~/composables/useAuth";
import {useUser} from "~/composables/useUser";

const publicRoutes: string[] = ["/", "/landing"];

const authRoutes: string[] = ["/login", "/register"];

//outside cause uknow 
export default defineNuxtRouteMiddleware((to,from)=> {
    const {isLoggedIn,user } = useAuth();
    console.log("hit hit hit hit")
    
    const isPublicRoute = publicRoutes.includes(to.path)
    

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
            return navigateTo("/home")
        }
        if (user.value?.isOnboarded){
            if (to.path === "/onboarding"){
                return navigateTo("/home")
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