import { type ChangeEvent, type FormEvent, useState } from 'react'
import { uploadMediaFile } from '#/queries/uploadQueries.ts'
import { useCreatePost } from '#/queries/postQueries.ts'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'
import { X } from 'lucide-react'
import {
    getGenresOptionsQuery,
    getInstrumentOptionsQuery,
} from '#/queries/OnboardingQueries.ts'
import { useQuery } from '@tanstack/react-query'

type CreatePostModalProps = {
    onClose: () => void
}

export function CreatePostmodal(props: CreatePostModalProps) {
    const genresQuery = useQuery(getGenresOptionsQuery)
    const instrumentsQuery = useQuery(getInstrumentOptionsQuery)

    const genresOptions = genresQuery.data ?? []
    const instrumentsOptions = instrumentsQuery.data ?? []

    const [selectedGenreIds, setSelectedGenres] = useState<string[]>([])
    const [selectedInstrumentIds, setSelectedInstruments] = useState<string[]>(
        [],
    )

    const [title, setTitle] = useState<string>('')
    const [description, setDescription] = useState<string>('')
    const [bandId, setBandId] = useState<string>('')

    const [isSubmitting, setIsSubmitting] = useState<boolean>(false)
    const [errorMessage, setErrorMessage] = useState<string>('')
    const [files, setFiles] = useState<File[]>([])

    const createPostMutation = useCreatePost()
    const handleRemoveFile = (index: number) => {
        setFiles((prev) => prev.filter((_, fIndex) => index !== fIndex))
    }

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault()
        if (!title.trim()) {
            return
        }
        setIsSubmitting(true)
        setErrorMessage('')

        try {
            const uploadIds: string[] = []

            for (const file of files) {
                const upload = await uploadMediaFile(file, 'Post')
                uploadIds.push(upload)
            }
            await createPostMutation.mutateAsync({
                title: title.trim(),
                description: description.trim(),
                uploadIds: uploadIds,
                genreIds: selectedGenreIds,
                instrumentIds: selectedInstrumentIds,
            })
            props.onClose()
        } catch (error) {
            setErrorMessage(getApiErrorMessage(error))
        } finally {
            setIsSubmitting(false)
        }
    }

    const handleAddGenre = (genreId: string) => {
        // meaning its already added
        if (!genreId || selectedGenreIds.includes(genreId)) {
            return
        }
        setSelectedGenres((prev) => [...prev, genreId])
    }

    const handleRemoveGenre = (genreId: string) => {
        setSelectedGenres((prev) => prev.filter((g) => g !== genreId))
    }

    const handleAddInstrument = (instrumentId: string) => {
        if (!instrumentId || selectedInstrumentIds.includes(instrumentId)) {
            return
        }
        setSelectedInstruments((prev) => [...prev, instrumentId])
    }
    const handleRemoveInstrument = (instrumentId: string) => {
        setSelectedInstruments((prev) => prev.filter((g) => g !== instrumentId))
    }
    const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
        if (!e.target.files) {
            return
        }

        const files = Array.from(e.target.files)

        setFiles((prev) => [...prev, ...files])

        // iamgine add guitar remove guitar add guitar
        // if e does not change then react does not run the handler
        e.target.value = ''
    }

    return (
        <div
            className="fixed inset-0 z-50 flex  items-center justify-center bg-black/60 p-4"
            onClick={() => {
                !isSubmitting && props.onClose()
            }}
        >
            <div
                className="relative w-full max-w-lg rounded-2xl bg-background-medium p-6 shadow-lg"
                onClick={(e) => e.stopPropagation()}
            >
                <form onSubmit={handleSubmit} className="flex flex-col gap-3 ">
                    <label>Title</label>
                    <input
                        type="text"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                    />

                    <label>Description</label>
                    <textarea
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        placeholder="Enter description..."
                        className="bg-background-high text-foreground-high"
                    />

                    <label>Genres</label>

                    <select
                        value=""
                        disabled={isSubmitting}
                        onChange={(e) => handleAddGenre(e.target.value)}
                    >
                        <option disabled value="">
                            Add a genre...
                        </option>
                        {genresOptions
                            .filter(
                                (go, i) => !selectedGenreIds.includes(go.id),
                            )
                            .map((go, i) => {
                                return (
                                    <option key={go.id} value={go.id}>
                                        {go.name}
                                    </option>
                                )
                            })}
                    </select>

                    {selectedGenreIds.map((genreId, i) => {
                        const genre = genresOptions.find(
                            (genre) => genre.id === genreId,
                        )

                        return (
                            <div key={genreId} className="flex">
                                <p> {genre?.name} </p>
                                <button
                                    disabled={isSubmitting}
                                    type="button"
                                    onClick={() => handleRemoveGenre(genreId)}
                                >
                                    <X />
                                </button>
                            </div>
                        )
                    })}

                    <label>Instruments</label>
                    <select
                        disabled={isSubmitting}
                        onChange={(e) => handleAddInstrument(e.target.value)}
                        value=""
                    >
                        <option disabled value="">
                            Select an instrument
                        </option>
                        {instrumentsOptions
                            .filter(
                                (ins, i) =>
                                    !selectedInstrumentIds.includes(ins.id),
                            )
                            .map((ins, i) => (
                                <option
                                    key={`options-${ins.id}`}
                                    value={ins.id}
                                >
                                    {' '}
                                    {ins.name}
                                </option>
                            ))}
                    </select>

                    {selectedInstrumentIds.map((insId, i) => {
                        const instrument = instrumentsOptions.find(
                            (inso, o) => inso.id === insId,
                        )
                        return (
                            <div key={insId} className="flex">
                                <p>{instrument?.name}</p>
                                <button
                                    disabled={isSubmitting}
                                    type="button"
                                    onClick={(e) =>
                                        handleRemoveInstrument(insId)
                                    }
                                >
                                    <X></X>
                                </button>
                            </div>
                        )
                    })}

                    <input
                        type="file"
                        multiple
                        disabled={isSubmitting}
                        accept="image/jpeg,image/png,audio/mpeg,audio/wav,video/mp4"
                        onChange={handleFileChange}
                    />

                    {files.length > 0 && (
                        <ul className="flex flex-col">
                            {files.map((file, i) => (
                                <li key={i} className="flex gap-2">
                                    <span>{file.name}</span>
                                    <button
                                        type="button"
                                        onClick={() => {
                                            if (!isSubmitting) {
                                                handleRemoveFile(i)
                                            }
                                        }}
                                    >
                                        <X />
                                    </button>
                                </li>
                            ))}
                        </ul>
                    )}

                    {errorMessage && (
                        <p className="text-sm text-red-500">{errorMessage}</p>
                    )}

                    <button
                        type="button"
                        disabled={isSubmitting}
                        onClick={props.onClose}
                        className="text-foreground-low absolute top-5 right-5"
                    >
                        <X />
                    </button>
                    <button
                        disabled={isSubmitting || !title.trim()}
                        type="submit"
                    >
                        {isSubmitting ? 'Posting' : 'Post'}
                    </button>
                </form>
            </div>
        </div>
    )
}
