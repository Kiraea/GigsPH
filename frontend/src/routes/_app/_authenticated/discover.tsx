import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/_app/_authenticated/discover')({
    component: RouteComponent,
})

function RouteComponent() {
    return <div>Hello "/_app/discover"!</div>
}
