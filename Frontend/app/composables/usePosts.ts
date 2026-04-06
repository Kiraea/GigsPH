import {postUtil} from "~/utils/postUtil";
import {useErrorHandler} from "~/composables/useErrorHandler";
import {refreshNuxtData} from "nuxt/app";


export const usePosts = () => {
    
    const {setErrors} = useErrorHandler();
    const {data: posts, pending: postsPending, error: postsError} = useFetch<GetPublicPostsResponse[]>('/api/posts', {
        key:'posts'
    })
    
    const isMutating = ref(false);
    
    const createPost = async (payload : CreatePostRequest) :Promise<CreatePostResponse> => {
        isMutating.value = true;
        try{
            const response = await postUtil.createPost(payload);
            await refreshNuxtData("posts");
            return response;
        }catch(e){
            setErrors(e)
            throw e  // the reason why thoriwng is because the pages should receive an error if fail or it will run the success portion only
        }finally {
            isMutating.value = false;
        }
    }
    
    return {posts, postsPending, postsError, createPost, isMutating}
    
}