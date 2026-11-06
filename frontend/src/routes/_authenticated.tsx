import { createFileRoute, Outlet, useLocation } from '@tanstack/react-router'
import { AuthContext } from '#/contexts/authContext.tsx'
import { useContext } from 'react'
import { Navigate } from '@tanstack/react-router'
export const Route = createFileRoute('/_authenticated')({
    component: AuthenticatedLayout,
})

function AuthenticatedLayout() {
    const auth = useContext(AuthContext)
    const pathname = useLocation({
        select: (location) => location.pathname,
    })

    if (auth?.isLoading) {
        return <p>Waiting...</p>
    } else {
        if (!auth?.user) {
            return <Navigate to="/login" replace />
        }
        if (!auth?.user.isOnboarded && pathname === '/onboarding') {
            return <Navigate to="/onboarding" replace />
        }
    }
    return <Outlet />
}
