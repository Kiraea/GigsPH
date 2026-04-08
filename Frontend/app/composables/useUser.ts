import { useErrorHandler } from "~/composables/useErrorHandler";

// Extended fields are nullable/empty because user may not have added them yet
export interface UserProfile {
    // Core required fields – backend will reject null/blank values
    displayName: string | null;
    firstName: string | null;
    lastName: string | null;
    description: string | null;
    location: LocationResponse | null;
    // Extended fields – can be null or empty arrays until user adds them
    socialLinks: SocialLinkResponse[] | null;
    genres: GenreResponse[] | null;
    instruments: InstrumentResponse[] | null;
}

export interface GenreResponse {
    id: string;
    name: string;
}

export interface InstrumentResponse {
    id: string;
    name: string;
}

export interface LocationResponse {
    id: string;
    country: string;
    city: string;
    provinceState: string;
}

export interface SocialLinkResponse {
    id: string;
    url: string;
}



export interface OnboardRequest{
    displayName: string,
    firstName: string,
    lastName: string,
    description: string,
    country: string,
    provinceState: string,
    city: string,
}


export interface OnboardResponse{

    displayName: string,
    firstName: string,
    lastName: string,
    description: string,
    location: LocationResponse 

    genres: GenreResponse[] | null
    instruments: InstrumentResponse [] | null , 
    socialLinks: SocialLinkResponse[] | null,
}

export const useUser = () => {
    const profile = useState<UserProfile | null>('user-profile', () => null);
    const { setErrors } = useErrorHandler();
    const isMutating = ref(false);

    const fetchFullProfile = async () => {
        try {
            const data = await $fetch<UserProfile>('/api/users/profile', {
                credentials:'include',
            });
            profile.value = data;
        } catch (e) {
            setErrors(e);
            throw e;
        }
    };

    const onboard = async (payload: OnboardRequest) => {
        isMutating.value = true;
        try {
            const response = await $fetch<OnboardResponse>("/api/users/onboard", {
                method: 'PUT',
                body: payload,
                credentials: 'include'
                }
            )
            // After onboarding, fetch full profile (extended fields will be null/empty)
            await fetchFullProfile();
        } catch (e) {
            setErrors(e);
        } finally {
            isMutating.value = false;
        }
    };

    const updateProfile = async (updatedProfile: UserProfile) => {
        isMutating.value = true;
        try {
            await $fetch('/api/users/profile', {
                method: 'PUT',
                body: updatedProfile,
                credentials: 'include',
            });
            // Refetch to get server‑validated version (avoids manual assignment)
            await fetchFullProfile();
        } catch (e) {
            setErrors(e);
        } finally {
            isMutating.value = false;
        }
    };

    return { profile, onboard, fetchFullProfile, updateProfile, isMutating };
};