import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/_app/messages')({
    component: RouteComponent,
})

function RouteComponent() {
    return <div>Hello "/messages"!</div>
}
