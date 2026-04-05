

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
    userId : string,
    displayName : string,
    createdAt: string,
    title:string,
    description: string,
    mediaUrl?: string | null,
    mediaType?: string | null,
    fileName?: string | null,
}



export const postUtil = {
    
    getPublicPosts: () :Promise<GetPublicPostsResponse> => {
        return (
            $fetch("/api/posts", {
                method: 'GET'
            })
        )
    },
    
    createPost: (payload:CreatePostRequest):Promise<CreatePostResponse> => 
        $fetch("/api/posts", {
            method:'POST',
        })
    
}