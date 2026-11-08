import { createFileRoute } from '@tanstack/react-router'
import { useEffect, useState } from 'react'
import { axiosInstance } from '#/axiosInstance.tsx'

import axios from 'axios'
export const Route = createFileRoute('/_authenticated/onboarding')({
    component: OnboardingPage,
})

type Platform = 'Facebook' | 'Twitter'

type SocialLink = {
    platform: Platform
    url: string
}

type Location = {
    city: string
    country: string
    provinceState: string
}

type Profile = {
    firstName: string
    lastName: string
    displayName: string
    description: string
    socialLinks: SocialLink[]
    location: Location
}

type Instruments = {
    id: string
    isMain: boolean
}

type StateSearchResponse = {
    results?: {
        state?: string
        place_id?: string
    }[]
}

type ProvinceStateOption = {
    state: string
    place_id: string
}
type CityResponse = {
    results?: {
        city?: string
    }[]
}

function OnboardingPage() {
    const step = ['Profile', 'Genres', 'Instruments', 'Review']
    const [currentStep, setCurrentStep] = useState(0)

    const [provinceStateOptions, setProvinceStateOptions] = useState<
        ProvinceStateOption[]
    >([])
    const [cityOptions, setCityOptions] = useState<string[]>([])

    const [profile, setProfile] = useState<Profile>({
        firstName: '',
        lastName: '',
        displayName: '',
        description: '',
        socialLinks: [], // can do [] as SocialLink[]
        location: {
            city: '',
            country: 'Philippines',
            provinceState: '',
        },
    })

    useEffect(() => {
        let ignoreResult = false
        const text = profile.location.provinceState.trim()
        if (text.length < 3) return

        const timer = setTimeout(async () => {
            const parameters = new URLSearchParams({
                text: text,
                type: 'state',
                limit: '5',
                filter: 'countrycode:ph',
                apiKey: import.meta.env.VITE_GEOAPIFY_API_KEY,
                format: 'json',
            })
            console.log(import.meta.env.VITE_GEOAPIFY_API_KEY)

            try {
                const response = await axios.get<StateSearchResponse>(
                    `https://api.geoapify.com/v1/geocode/autocomplete?${parameters}`,
                )
                console.log(response.data.results)
                const availableStates = (response.data.results ?? []).filter(
                    (row): row is ProvinceStateOption =>
                        typeof row.state === 'string' &&
                        row.state.length > 0 &&
                        typeof row.place_id === 'string' &&
                        row.place_id.length > 0,
                )

                /*
                
                              const availableStates = (response.data.results ?? [])
                    .map((row) => row.state)
                    .filter(
                        (name): name is string =>
                            typeof name === 'string' && name.length > 0,
                    )
                 */
                if (!ignoreResult) {
                    setProvinceStateOptions(availableStates)
                }
                // ... new Set(availableStates) to remove dups
            } catch (e) {
                console.log(e)
            }
        }, 2000)

        return () => {
            clearTimeout(timer)
            ignoreResult = true
        }
    }, [profile.location.provinceState])

    const provinceStateValid = provinceStateOptions.some(
        (province) => province.state === profile.location.provinceState,
    )

    useEffect(() => {
        let ignoreResult = false
        const text = profile.location.city.trim()
        if (text.length < 3) return
        if (!provinceStateValid) {
            return
        }
        const provinceObject = provinceStateOptions.find(
            (p) => p.state === profile.location.provinceState,
        )
        if (!provinceObject) {
            return
        }
        const timer = setTimeout(async () => {
            const parameters = new URLSearchParams({
                text: text,
                type: 'city',
                limit: '5',
                filter: `place:${provinceObject.place_id}`,
                apiKey: import.meta.env.VITE_GEOAPIFY_API_KEY,
                format: 'json',
            })

            try {
                const response = await axios.get<CityResponse>(
                    `https://api.geoapify.com/v1/geocode/autocomplete?${parameters}`,
                )
                console.log(response.data.results)
                const availableCities = (response.data.results ?? [])
                    .map((row) => row.city)
                    .filter(
                        (name): name is string =>
                            typeof name === 'string' && name.length > 0,
                    )
                if (!ignoreResult) {
                    setCityOptions(availableCities)
                }
                // ... new Set(availableStates) to remove dups
            } catch (e) {
                console.log(e)
            }
        }, 2000)

        return () => {
            clearTimeout(timer)
            ignoreResult = true
        }
    }, [
        profile.location.city,
        profile.location.provinceState,
        provinceStateValid,
    ])

    const [genres, setGenres] = useState<string[]>([])
    const [instruments, setInstruments] = useState<Instruments[]>([])

    return (
        <div className="flex items-center justify-center min-h-screen">
            <div className="flex flex-col w-1/2 gap-2 ">
                <div className="flex gap-2 border-black p-5 border-2">
                    {step.map((stepName, index) => (
                        <button
                            key={stepName}
                            type="button"
                            className="flex-1 rounded border p-3"
                            onClick={() => setCurrentStep(index)}
                        >
                            {stepName}
                        </button>
                    ))}
                </div>
                <div className="flex flex-col border-2 border-blac min-h-96 p-6">
                    {currentStep === 0 && (
                        <>
                            <label>Firstname</label>
                            <input
                                type="text"
                                value={profile.firstName}
                                onChange={(e) =>
                                    setProfile((previous) => {
                                        return {
                                            ...previous,
                                            firstName: e.target.value,
                                        }
                                    })
                                }
                            />

                            <label>Lastname</label>
                            <input
                                type="text"
                                value={profile.lastName}
                                onChange={(e) =>
                                    setProfile((previous) => {
                                        return {
                                            ...previous,
                                            lastName: e.target.value,
                                        }
                                    })
                                }
                            />

                            <label>DisplayName</label>
                            <input
                                type="text"
                                value={profile.displayName}
                                onChange={(e) =>
                                    setProfile((previous) => {
                                        return {
                                            ...previous,
                                            displayName: e.target.value,
                                        }
                                    })
                                }
                            />

                            <label>Description</label>
                            <input
                                type="text"
                                value={profile.description}
                                onChange={(e) =>
                                    setProfile((previous) => {
                                        return {
                                            ...previous,
                                            description: e.target.value,
                                        }
                                    })
                                }
                            />
                            <label>City</label>

                            <input
                                type="text"
                                list="city-options"
                                value={profile.location.city}
                                disabled={!provinceStateValid}
                                onChange={(e) =>
                                    setProfile((previous) => {
                                        return {
                                            ...previous,
                                            location: {
                                                ...previous.location,
                                                city: e.target.value,
                                            },
                                        }
                                    })
                                }
                            />
                            <datalist id="city-options">
                                {cityOptions.map((option) => (
                                    <option key={option} value={option}>
                                        {option}
                                    </option>
                                ))}
                            </datalist>

                            <label>Province/State</label>
                            <input
                                list="province-state-options"
                                type="text"
                                value={profile.location.provinceState}
                                onChange={(e) =>
                                    setProfile((previous) => {
                                        return {
                                            ...previous,
                                            location: {
                                                ...previous.location,
                                                provinceState: e.target.value,
                                            },
                                        }
                                    })
                                }
                            />
                            <datalist id="province-state-options">
                                {provinceStateOptions.map((option) => {
                                    return (
                                        <option
                                            key={option.state}
                                            value={option.state}
                                        >
                                            {option.state}
                                        </option>
                                    )
                                })}
                            </datalist>

                            <label>Country</label>
                            <input
                                type="text"
                                value={profile.location.country}
                                readOnly
                            />
                        </>
                    )}
                    {currentStep === 1 && <div>Genres</div>}
                    {currentStep === 2 && <div>Instruments </div>}
                    {currentStep === 3 && <div>Review </div>}
                </div>
            </div>
        </div>
    )
}
