
export interface OnboardPayload {
    displayName: string,
    firstName: string,
    lastName: string,
    description: string,
}


export interface OnboardResponse{

    displayName: string,
    firstName: string,
    lastName: string,
    description: string,
}
// rmemeber no need to put credentials
// because since browser and nuxtserver are in the same origin it uauto sends it already so uknow

export const userUtil = {

    onboard: (payload: OnboardPayload) :Promise<OnboardResponse> =>
        $fetch("/api/users/onboard", {
            method: 'PUT',
            body: payload,}
        )
}