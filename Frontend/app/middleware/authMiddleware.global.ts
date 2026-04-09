import { useAuth } from "~/composables/useAuth";

// Only these EXACT pages are public
const publicRouteNames: string[] = ["index", "landing", "profile-id"];

// Only these EXACT pages are for guests (people not logged in)
const authRouteNames: string[] = ["login", "register"];

export default defineNuxtRouteMiddleware((to, from) => {
    const { isLoggedIn, user } = useAuth();

    // Cast to string safely
    const routeName = String(to.name);

    const isPublicRoute = publicRouteNames.includes(routeName);
    const isAuthRoute = authRouteNames.includes(routeName);
    console.log(isLoggedIn.value)
    console.log(user.value?.isOnboarded)
    // 1. If NOT logged in
    if (!isLoggedIn.value) {
        if (!isPublicRoute && !isAuthRoute) {
            return navigateTo("/login");
        }
        return; // Let them pass
    }

    // 2. If LOGGED in
    if (isLoggedIn.value) {
        if (isAuthRoute) {
            return navigateTo("/home");
        }
        /*
        if (routeName === "profile-id" && to.params.id === user.value?.id){
            return navigateTo("/profile/me");
        }
        */
        // Onboarding logic
        if (user.value?.isOnboarded) {
            if (to.path === "/onboarding") {
                return navigateTo("/home");
            }
            return;
        } else {
            if (to.path !== "/onboarding") {
                return navigateTo("/onboarding");
            }
            return;
        }
    }
});