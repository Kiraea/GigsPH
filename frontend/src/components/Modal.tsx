import { type ReactNode, useEffect } from 'react'

type ModalProps = {
    onClose: () => void
    children: ReactNode
    closeDisabled?: boolean
    className?: string
}

export function Modal(props: ModalProps) {
    const canClose = !props.closeDisabled

    useEffect(() => {
        const handleKeyDown = (e: KeyboardEvent) => {
            if (e.key === 'Escape' && canClose) {
                props.onClose()
            }
        }

        window.addEventListener('keydown', handleKeyDown)
        return () => {
            window.removeEventListener('keydown', handleKeyDown)
        }
    }, [canClose, props.onClose])

    return (
        <div
            className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4"
            onClick={() => {
                if (canClose) {
                    props.onClose()
                }
            }}
        >
            <div
                className={`relative flex w-full flex-col gap-4 rounded-2xl p-6 text-foreground-high shadow-lg ${props.className ?? 'max-w-md bg-background-high'}`}
                onClick={(e) => e.stopPropagation()}
            >
                {props.children}
            </div>
        </div>
    )
}
