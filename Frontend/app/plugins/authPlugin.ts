import { useAuth, type CheckTokenResponse } from '~/composables/useAuth';

export default defineNuxtPlugin(async () => {
    const { setUser, isLoading } = useAuth();
    isLoading.value = true;
    try {
        const { data } = await useFetch<CheckTokenResponse>('/api/auth/check-token', {
            headers: useRequestHeaders(['cookie']) as HeadersInit,
        });
        if (data.value) {
            setUser(data.value.userId, data.value.isOnboarded);
        } else {
            setUser(null);
        }
    } finally {
        isLoading.value = false;
    }
});