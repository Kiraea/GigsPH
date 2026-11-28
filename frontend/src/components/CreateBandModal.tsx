import { useState, type SubmitEvent } from 'react'
import { useCreateBand } from '#/queries/bandQueries.tsx'
import { getApiErrorMessage } from '#/utils/getApiErrorMessage.ts'
import { Modal } from '#/components/Modal.tsx'

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
        <Modal onClose={props.onClose} closeDisabled={createBand.isPending}>
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
        </Modal>
    )
}
