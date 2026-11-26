import { useState } from 'react'
import { useCreateBand } from '#/queries/bandQueries.tsx'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'
import { type SubmitEvent } from 'react'

type CreateBandModalProps = {
    onClose: () => void
}

export function CreateBandModal(props: CreateBandModalProps) {
    const [name, setName] = useState('')
    const [description, setDescription] = useState('')

    const createBand = useCreateBand()

    const handleSubmit = (e: SubmitEvent<HTMLFormElement>) => {
        e.preventDefault()
        if (!name.trim()) {
            return
        }
        createBand.mutate(
            { name: name.trim(), description: description.trim() || null },
            {
                onSuccess: () => {
                    props.onClose()
                },
            },
        )
    }

    return (
        <div
            className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center "
            onClick={() => !createBand.isPending && props.onClose()}
        >
            <div
                className="bg-background-high w-full max-w-md p-6 text-foreground-high"
                onClick={(e) => e.stopPropagation()}
            >
                <h2 className="text-md font-bold">Create Band</h2>
                <button
                    type="button"
                    onClick={() => {
                        props.onClose()
                    }}
                    disabled={createBand.isPending}
                ></button>

                <form onSubmit={handleSubmit} className="flex flex-col gap-2">
                    <label>Name</label>
                    <input
                        type="text"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />

                    <label>Description</label>
                    <textarea
                        value={description}
                        className="rounded-md border border-foreground-high"
                        onChange={(e) => setDescription(e.target.value)}
                    ></textarea>

                    {createBand.isError && (
                        <div>{getApiErrorMessage(createBand.error)}</div>
                    )}

                    <button
                        type="submit"
                        disabled={createBand.isPending || !name.trim()}
                    >
                        {createBand.isPending ? 'Creating...' : 'Create Band'}
                    </button>
                </form>
            </div>
        </div>
    )
}
