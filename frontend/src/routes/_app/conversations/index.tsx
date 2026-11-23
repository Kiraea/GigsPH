import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/_app/conversations/')({
    component: RouteComponent,
})

function RouteComponent() {
    return (
        <div className="flex items-center justify-center h-full">
            <label className="text-7xl font-bold">
                No Conversation Selected
            </label>
        </div>
    )
}
