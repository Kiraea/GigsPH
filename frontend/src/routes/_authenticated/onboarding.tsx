import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/_authenticated/onboarding')({
  component: RouteComponent,
})

function RouteComponent() {
  return <div>Hello "/onboarding"!</div>
}
