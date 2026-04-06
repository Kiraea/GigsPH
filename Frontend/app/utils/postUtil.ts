

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
    
}

export interface UpdatePostResponse{

}


export const postUtil = {
    

    createPost: (payload:CreatePostRequest):Promise<CreatePostResponse> => {
        const formData = new FormData
        
        formData.append("title", payload.title)
        formData.append("description", payload.title)
        if(payload.file){

            formData.append("file", payload.file)
        } 
        
        return ($fetch("/api/posts", {
            method:'POST',
            body:formData
        }))
    },

    
    UpdatePost: (payload:UpdatePostRequest): Promise<UpdatePostResponse> => 
        $fetch(`/api/posts/${id}`, {
            method:formdata
        })
    
    /*
    
    createPost: (payload: CreatePostRequest): Promise<CreatePostResponse> => {
    // 1. Create a new FormData object
    const formData = new FormData();

    // 2. Append the fields to the FormData
    // Note: We check if they exist first, so we don't append "undefined" or "null" strings
    if (payload.title) {
        formData.append('title', payload.title);
    }
    if (payload.description) {
        formData.append('description', payload.description);
    }
    if (payload.file) {
        formData.append('file', payload.file); 
    }

    // 3. Send the FormData as the body
    return $fetch("/api/posts", {
        method: 'POST',
        body: formData
    });
}
     */
        
    
    
}