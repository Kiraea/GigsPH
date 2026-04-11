import {useErrorHandler} from "~/composables/useErrorHandler";
import {refreshNuxtData} from "nuxt/app";



export interface CreatePostRequest {
    title : string,
    description : string,
    file? : File | null
}


export interface CreatePostResponse{
    title : string,
    description : string,
    url? : string | null,
    mediaType?: string | null,
    fileName? : string | null,
}



export interface GetPublicPostsResponse{
    id: string,
    userId : string,
    displayName : string,
    createdAt: string,
    title:string,
    description: string,
    mediaUrl?: string | null,
    mediaType?: string | null,
    fileName?: string | null,
}

export interface GetPublicPostsWrapperResponse{
    posts: GetPublicPostsResponse[],
    hasNextPage: boolean
}




export interface UpdatePostRequest {
    title?: string | null,
    description?: string | null,
    file?: File | null,
    removeMedia: boolean,
} 

export interface UpdatePostResponse{

    id: string,
    userId: string,
    title: string,
    description: string,
    mediaType?: string | null,
    fileName? : string | null,
}


// usePosts.ts - list fetch only
export const usePosts = () => {
    const {setErrors} = useErrorHandler()
    
    const Posts = ref<GetPublicPostsResponse[]>([])
    const limit = 5
    const page =  ref(1);
    const hasNextPage = ref(true);
    const isPending = ref(false);
    
    const { data: InitialPosts, pending:InitialPostsPending,  error: InitialPostsError } = useFetch<GetPublicPostsWrapperResponse>(
        `/api/posts/?page=${page.value}&limit=${limit}`, {
            onResponseError({response}){
                console.log(response._data)
                setErrors(response._data)
            },
            lazy: import.meta.client,
        });
        
    watch(InitialPosts, (newPosts) => {
        if (!newPosts) return;
        Posts.value=newPosts.posts;
        hasNextPage.value = newPosts?.hasNextPage;
        if(hasNextPage){
            page.value++;
        }
    })

    const isLoading = computed(()=> isPending.value || InitialPostsPending.value)
    
    
    const fetchMorePosts = async () => {
        try{
            const result = await $fetch<GetPublicPostsWrapperResponse>(`/api/posts/?page=${page.value}&limit=${limit}`, {
            })    
            Posts.value?.push(...result.posts) 
            hasNextPage.value = result.hasNextPage
            if (hasNextPage.value){
                page.value++;
            }
        }catch(e){
            setErrors(e)
        }
    }
    
    
};


export const usePost = (postId: string) => {
    const { data: post, pending: postPending, error: postError } = useFetch<GetPublicPostsResponse>(
        `/api/posts/${postId}`,
        { key: `post-${postId}` }
    );
    return { post, postPending, postError};
};

// usePostMutations.ts - mutations only, no fetch
export const usePostMutations = () => {
    const { setErrors } = useErrorHandler();
    const isMutating = ref(false);

    const createPost = async (payload: CreatePostRequest) => {
        isMutating.value = true;
        try {
            const formData = new FormData();
            formData.append('title', payload.title);
            formData.append('description', payload.description);
            if (payload.file) formData.append('file', payload.file);

            await $fetch<GetPublicPostsResponse>('/api/posts', {
                method: 'POST',
                body: formData,
                credentials: 'include',
            });
        } catch (e) {
            setErrors(e);
            throw e;
        } finally {
            isMutating.value = false;
        }
    };

    const updatePost = async (postId: string, payload: UpdatePostRequest) => {
        isMutating.value = true;
        console.log(postId)
        try {
            const formData = new FormData();
            if (payload.title) formData.append('title', payload.title);
            if (payload.description) formData.append('description', payload.description);
            if (payload.file) formData.append('file', payload.file);
            formData.append('removeMedia', String(payload.removeMedia));

            await $fetch(`/api/posts/${postId}`, {
                method: 'PATCH',
                body: formData,
                credentials: 'include',
            });
            await Promise.all([
                refreshNuxtData('posts'),
                refreshNuxtData(`post-${postId}`)
            ]);
        } catch (e) {
            setErrors(e);
            throw e;
        } finally {
            isMutating.value = false;
        }
    };

    const deletePost = async (postId: string) => {
        isMutating.value = true;
        try {
            await $fetch(`/api/posts/${postId}`, {
                method: 'DELETE',
                credentials: 'include',
            });
            await Promise.all([
                refreshNuxtData('posts'),
                refreshNuxtData(`post-${postId}`)
            ]);
        } catch (e) {
            setErrors(e);
            throw e;
        } finally {
            isMutating.value = false;
        }
    };

    return { createPost, updatePost, deletePost, isMutating };
};

