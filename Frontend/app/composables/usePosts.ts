import {postUtil} from "~/utils/postUtil";
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
    Id: string,
    userId : string,
    displayName : string,
    createdAt: string,
    title:string,
    description: string,
    mediaUrl?: string | null,
    mediaType?: string | null,
    fileName?: string | null,
}
export interface UpdatePostRequest {
    postId: string,
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


export const usePosts = () => {
    
    const {setErrors} = useErrorHandler();
    const {data: posts, pending: postsPending, error: postsError} = useFetch<GetPublicPostsResponse[]>('/api/posts', {
        key:'posts'
    })
    
    const isMutating = ref(false);
    
    const createPost = async (payload : CreatePostRequest)  => {
        isMutating.value = true;
        try{
            const formData = new FormData
            formData.append("title", payload.title)
            formData.append("description", payload.description)
            if(payload.file){
                formData.append("file", payload.file)
            }
            const response = $fetch<GetPublicPostsResponse>("/api/posts", {
                method:'POST',
                body:formData,
                credentials: 'include'
            })
            await refreshNuxtData("posts");
        }catch(e){
            setErrors(e)
            throw e  // the reason why thoriwng is because the pages should receive an error if fail or it will run the success portion only
        }finally {
            isMutating.value = false;
        }
    }


    const updatePost = async (payload: UpdatePostRequest) => {
        try{

            const formData = new FormData;
            formData.append("postId", payload.postId)
            
            if (payload.title) formData.append("title", payload.title)
            
            if(payload.description) formData.append("description", payload.description)
            if (payload.file) formData.append("file", payload.file)
             formData.append("removeMedia", String(payload.removeMedia))
            
            const response = await $fetch("/api/posts", {
                method:'PATCH',
                body: formData,
                credentials: 'include'
            })
            await refreshNuxtData("posts");
        }catch(e){
            setErrors(e)
            throw e
        }
    }
    
    
    return {posts, postsPending, postsError, createPost, isMutating, updatePost}
    
}