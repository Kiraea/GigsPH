import { useErrorHandler } from "~/composables/useErrorHandler";
import {useFetch} from "nuxt/app";

// Extended fields are nullable/empty because user may not have added them yet

export interface  GetPublicProfileResponse{

    // Core required fields – backend will reject null/blank values
    Id: string;
    displayName?: string | null;
    firstName?: string | null;
    lastName?: string | null;
    description?: string | null;
    location?: LocationResponse | null;
    // Extended fields – can be null or empty arrays until user adds them
    socialLinks: SocialLinkResponse[] | null;
    genres: GenreResponse[] | null;
    instruments: InstrumentResponse[] | null;
}
export interface UpdateProfileRequest{

    // Core required fields – backend will reject null/blank values
    description?: string | null;
    // Extended fields – can be null or empty arrays until user adds them
    socialLinks: string[] | null;
    genresIds: string[] | null;
    instrumentsIds: string[] | null;
    country: string;
    city: string;
    provinceState: string;
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
    location: LocationResponse,
    genres: GenreResponse[] | null,
    instruments: InstrumentResponse [] | null , 
    socialLinks: SocialLinkResponse[] | null,
}



export const useProfileMutations = () => {
    const { setErrors } = useErrorHandler();
    const { user } = useAuth();
    const isMutating = ref(false);

    const onboard = async (payload: OnboardRequest) => {
        isMutating.value = true;
        try {
            await $fetch<OnboardResponse>('/api/users/onboard', {
                method: 'PUT',
                body: payload,
                credentials: 'include',
            });
            if (user.value){
                user.value.isOnboarded = true
            }
            await refreshNuxtData(`user-${user.value?.id}`);
        } catch (e) {
            setErrors(e);
            throw e;
        } finally {
            isMutating.value = false;
        }
    };

    const updateProfile = async (payload: UpdateProfileRequest) => {
        isMutating.value = true;
        try {
            await $fetch('/api/users/me', {
                method: 'PATCH',
                body: payload,
                credentials: 'include',
            });
            await refreshNuxtData(`user-${user.value?.id}`);
        } catch (e) {
            setErrors(e);
            throw e;
        } finally {
            isMutating.value = false;
        }
    };

    return { onboard, updateProfile, isMutating };
};


export const useUser = (userId: string) => {
    const { setErrors } = useErrorHandler();
    const { data: pubUser, pending: pubUserPending, error: pubUserError } = useFetch<GetPublicProfileResponse>(
        `/api/users/${userId}`,
        {
            key: `user-${userId}`,
            onResponseError({ response }) {
                console.log(response);
                if (import.meta.client) {
                    setErrors(response._data);
                }
            }
        }
    );
    return { pubUser, pubUserPending, pubUserError };
};