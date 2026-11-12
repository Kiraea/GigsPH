import { createFileRoute, ErrorComponent } from '@tanstack/react-router'
import { useEffect, useState } from 'react'
import { axiosInstance } from '#/axiosInstance.tsx'
import { QueryClient, useQuery } from '@tanstack/react-query'
import axios from 'axios'
import {
    type GenresOptions,
    getGenresOptionsQuery,
    getInstrumentOptionsQuery,
    type InstrumentOptions,
} from '#/queries/OnboardingQueries.ts'
export const Route = createFileRoute('/_authenticated/onboarding')({
    component: OnboardingPage,
    loader: async ({ context }) => {
        await Promise.all([
            context.queryClient.query(getGenresOptionsQuery),
            context.queryClient.query(getInstrumentOptionsQuery),
        ])
    },
    pendingComponent: () => <p>Loading Existing Genres and Instruments</p>,
    errorComponent: () => <p>Something went wrong</p>,
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
    latitude: number | undefined
    longitude: number | undefined
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
        lat: number
        lon: number
    }[]
}

type CityOption = {
    city: string
    lat: number
    lon: number
}

function OnboardingPage() {
    const step = ['Profile', 'Genres', 'Instruments', 'Review']
    const [currentStep, setCurrentStep] = useState(0)

    const { data: genresData = [] } = useQuery(getGenresOptionsQuery)
    const { data: instrumentsData = [] } = useQuery(getInstrumentOptionsQuery)

    const [genres, setGenres] = useState<string[]>([''])
    const [instruments, setInstruments] = useState<Instruments[]>([
        {
            id: '',
            isMain: false,
        },
    ])

    const changeInstrument = (
        rowIndex: number,
        selectedId: string,
        isMain: boolean,
    ) => {
        setInstruments((prev) => {
            const updated = [...prev]
            // reason why [...] is because 1. u cant edit state directly, [.. allows for creation of new array diff memory
            updated[rowIndex] = {
                id: selectedId,
                isMain: isMain,
            }
            return updated
        })
    }

    const [provinceStateOptions, setProvinceStateOptions] = useState<
        ProvinceStateOption[]
    >([])
    const [cityOptions, setCityOptions] = useState<CityOption[]>([])

    const changeCity = (city: string) => {
        const getCityProperties = cityOptions.find(
            (cityOption) => cityOption.city === city,
        )

        setProfile((prev) => {
            return {
                ...prev,
                location: {
                    ...prev.location,
                    city: city,
                    latitude: getCityProperties?.lat,
                    longitude: getCityProperties?.lon,
                },
            }
        })
    }

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
            latitude: undefined,
            longitude: undefined,
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
                const availableCities = (response.data.results ?? []).filter(
                    (row): row is CityOption =>
                        typeof row.city === 'string' &&
                        row.city.length > 0 &&
                        typeof row.lat === 'number' &&
                        typeof row.lon === 'number',
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

    function changeGenre(rowIndex: number, selectedId: string) {
        setGenres((prev) =>
            prev.map((genreId, index) =>
                index === rowIndex ? selectedId : genreId,
            ),
        )
    }

    const selectedGenres = genres.filter((id) => id !== '')
    const selectedInstruments = instruments.filter(
        (instrument) => instrument.id !== '',
    )

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
                <div className="flex flex-col border-2 border-black min-h-96 p-6">
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
                                onChange={(e) => changeCity(e.target.value)}
                            />
                            <datalist id="city-options">
                                {cityOptions.map((option) => (
                                    <option
                                        key={option.city}
                                        value={option.city}
                                    >
                                        {option.city}
                                    </option>
                                ))}
                            </datalist>

                            <label>Province/State</label>
                            <input
                                list="province-state-options"
                                type="text"
                                value={profile.location.provinceState}
                                onChange={(e) =>
                                {
                                    setCityOptions([])
                                    setProfile((previous) => {
                                        return {
                                            ...previous,
                                            location: {
                                                ...previous.location,
                                                provinceState: e.target.value,
                                                city: '',
                                                latitude: undefined,
                                                longitude: undefined,
                                            },
                                        }
                                    })
                                }
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
                    {currentStep === 1 && (
                        <>
                            {genres.map((genreId, index) => (
                                <div key={index} className="flex flex-col">
                                    <label> Genre {index}</label>
                                    <select
                                        id={`genre-${genreId}`}
                                        value={genreId}
                                        onChange={(e) => {
                                            changeGenre(index, e.target.value)
                                        }}
                                    >
                                        <option value="" disabled>
                                            Select a Genre
                                        </option>
                                        {genresData.map((genre) => (
                                            <option
                                                key={`option-${genre.id}`}
                                                value={genre.id}
                                            >
                                                {genre.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            ))}
                            <div className="flex  gap-2 justify-end w-full">
                                <button
                                    onClick={(e) => {
                                        e.preventDefault()
                                        if (genres.length < 5) {
                                            setGenres((prev) => [...prev, ''])
                                        }
                                    }}
                                    className="bg-green-300 "
                                >
                                    Add Genre +
                                </button>

                                <button
                                    onClick={(e) => {
                                        e.preventDefault()
                                        if (genres.length > 1) {
                                            setGenres((prev) =>
                                                prev.slice(0, -1),
                                            )
                                        }
                                    }}
                                    className="bg-green-300 "
                                >
                                    Remove -
                                </button>
                            </div>
                        </>
                    )}

                    {currentStep === 2 && (
                        <>
                            <label>Select what instrument you play</label>

                            {instruments.map((instrument, index) => (
                                <div key={`instrument-${index}`}>
                                    <label>Instrument {index}</label>
                                    <div className="flex gap-2">
                                        <select
                                            value={instrument.id}
                                            onChange={(e) =>
                                                changeInstrument(
                                                    index,
                                                    e.target.value,
                                                    false,
                                                )
                                            }
                                        >
                                            <option value="" disabled>
                                                Select an instrument
                                            </option>

                                            {instrumentsData.map(
                                                (instrument) => (
                                                    <option
                                                        key={instrument.id}
                                                        value={instrument.id}
                                                    >
                                                        {instrument.name}
                                                    </option>
                                                ),
                                            )}
                                        </select>
                                        <input
                                            type="checkbox"
                                            checked={instrument.isMain}
                                            onChange={(e) =>
                                                changeInstrument(
                                                    index,
                                                    instrument.id,
                                                    e.target.checked,
                                                )
                                            }
                                        />
                                    </div>
                                </div>
                            ))}

                            <div className="flex gap-2 justify-end w-full">
                                <button
                                    onClick={(e) => {
                                        e.preventDefault()
                                        if (instruments.length < 4) {
                                            setInstruments((prev) => [
                                                ...prev,
                                                {
                                                    id: '',
                                                    isMain: false,
                                                },
                                            ])
                                        }
                                    }}
                                    className="bg-green-300"
                                >
                                    Add Instrument +
                                </button>

                                <button
                                    onClick={(e) => {
                                        e.preventDefault()
                                        if (instruments.length > 1) {
                                            setInstruments((prev) =>
                                                prev.slice(0, -1),
                                            )
                                        }
                                    }}
                                    className="bg-green-300"
                                >
                                    Remove +
                                </button>
                            </div>
                        </>
                    )}
                    {currentStep === 3 && (
                        <section className="flex flex-col gap-4">
                            <h2>Review your information</h2>
                            <dl className="grid grid-cols-[auto_1fr] gap-x-6 gap-y-3">
                                <dt>First name</dt>
                                <dd>
                                    {profile.firstName.trim() || 'Not provided'}
                                </dd>

                                <dt>Last name</dt>
                                <dd>
                                    {profile.lastName.trim() || 'Not provided'}
                                </dd>

                                <dt>Display name</dt>
                                <dd>
                                    {profile.displayName.trim() ||
                                        'Not provided'}
                                </dd>

                                <dt>Description</dt>
                                <dd className="whitespace-pre-wrap break-words">
                                    {profile.description.trim() ||
                                        'Not provided'}
                                </dd>

                                <dt>Location</dt>
                                <dd>
                                    {[
                                        profile.location.city,
                                        profile.location.provinceState,
                                        profile.location.country,
                                    ]
                                        .filter((part) => part.trim() !== '')
                                        .join(', ') || 'Not provided'}
                                </dd>

                                <dt>Genres</dt>
                                <dd>
                                    {selectedGenres.length > 0 ? (
                                        <ul>
                                            {selectedGenres.map((id, index) => (
                                                <li key={`${id}-${index}`}>
                                                    {genresData.find(
                                                        (genre) =>
                                                            genre.id === id,
                                                    )?.name || 'Unknown genre'}
                                                </li>
                                            ))}
                                        </ul>
                                    ) : (
                                        'None selected'
                                    )}
                                </dd>

                                <dt>Instruments</dt>
                                <dd>
                                    {selectedInstruments.length > 0 ? (
                                        <ul>
                                            {selectedInstruments.map(
                                                (instrument, index) => (
                                                    <li
                                                        key={`${instrument.id}-${index}`}
                                                    >
                                                        {instrumentsData.find(
                                                            (option) =>
                                                                option.id ===
                                                                instrument.id,
                                                        )?.name ||
                                                            'Unknown instrument'}
                                                        {instrument.isMain &&
                                                            ' (Main)'}
                                                    </li>
                                                ),
                                            )}
                                        </ul>
                                    ) : (
                                        'None selected'
                                    )}
                                </dd>

                                <dt>Social links</dt>
                                <dd>
                                    {profile.socialLinks.length > 0 ? (
                                        <ul>
                                            {profile.socialLinks.map(
                                                (link, index) => (
                                                    <li
                                                        key={`${link.platform}-${index}`}
                                                    >
                                                        {link.platform}:{' '}
                                                        {link.url}
                                                    </li>
                                                ),
                                            )}
                                        </ul>
                                    ) : (
                                        'None added'
                                    )}
                                </dd>
                            </dl>
                        </section>
                    )}
                </div>
            </div>
        </div>
    )
}
