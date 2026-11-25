import axios from 'axios'

type ProblemDetails = {
    errors?: Record<string, string[]>
    title?: string
    detail?: string
}

export function getApiErrorMessage(error: unknown) {
    if (axios.isAxiosError<ProblemDetails>(error)) {
        const data = error.response?.data

        if (data?.errors) {
            const messages = Object.values(data.errors).flat()
            if (messages.length > 0) {
                return messages.join(' ')
            }
        }
        if (data?.detail) {
            return data.detail
        }

        if (data?.title) {
            return data.title
        }

        if (!error.response) {
            return 'Cannot reach the server'
        }
    }
    return 'unexpected error occured.'
}
