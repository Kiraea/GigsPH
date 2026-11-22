import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/_app/music-listings')({
    component: RouteComponent,
})

function RouteComponent() {
    return <div>Hello "/_app/music-listings"!</div>
}
