import { axiosInstance } from '#/axiosInstance.tsx'
import axios from 'axios'

export type UploadPurpose = 'Post' | 'Profile'

export type UploadRequest = {
    fileName: string
    fileSize: number
    fileType: string
    purpose: UploadPurpose
}

export type UploadResponse = {
    uploadId: string
    uploadUrl: string
}

async function requestUploadUrl(
    request: UploadRequest,
): Promise<UploadResponse> {
    const response = await axiosInstance.post<UploadResponse>(
        '/upload',
        request,
    )
    return response.data
}

export async function uploadMediaFile(
    file: File,
    purpose: UploadPurpose,
): Promise<string> {
    const { uploadId, uploadUrl } = await requestUploadUrl({
        fileName: file.name,
        fileSize: file.size,
        fileType: file.type,
        purpose: purpose,
    })

    await axios.put(uploadUrl, file, {
        headers: {
            'Content-Type': file.type,
        },
    })

    return uploadId
}
