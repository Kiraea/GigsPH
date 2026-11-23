import { createFileRoute, Link, Outlet } from '@tanstack/react-router'
import { useLocation } from '@tanstack/react-router'
import { Handshake, House, MessagesCircle, Network, Search } from 'lucide-react'
export const Route = createFileRoute('/_app')({
    component: AppLayout,
})

const sidebarLinkClass =
    'col-span-2 grid grid-cols-subgrid items-center rounded-2xl p-2'

function AppLayout() {
    const pathname = useLocation({
        select: (location) => location.pathname,
    })
    if (pathname == '/onboarding') {
        return <Outlet />
    }

    return (
        <div className="grid-cols-[280px_minmax(0,1fr)] grid min-h-screen bg-background-high">
            <nav className="sticky self-start bg-background-high h-dvh gap-10 top-0 col-start-1 flex-col flex p-3 ">
                <h1 className="text-4xl font-medium">
                    {' '}
                    SessionPH{' '}
                    <img src="/media/logo.png" className="h-10 w-10" />
                </h1>

                <div className="grid grid-cols-[auto_1fr]  gap-x-3 gap-y-2 w-full font-light text-foreground-medium text-md ">
                    <Link
                        to="/feed"
                        className={`${sidebarLinkClass} bg-purple-200`}
                    >
                        <span className="flex items-center gap-2">
                            <House />
                        </span>
                        <span>Feed</span>
                    </Link>
                    <Link to="/conversations" className={sidebarLinkClass}>
                        <span className="flex items-center gap-2">
                            <MessagesCircle />
                        </span>
                        <span>Messages</span>
                    </Link>
                    <Link to="/discover" className={sidebarLinkClass}>
                        <span className="flex items-center gap-2">
                            <Search />
                        </span>
                        <span>Discover</span>
                    </Link>
                    <Link to="/music-listings" className={sidebarLinkClass}>
                        <span className="flex items-center gap-2">
                            <Network />
                        </span>
                        <span>Music Listings</span>
                    </Link>
                    <Link to="/bands" className={sidebarLinkClass}>
                        <span className="flex items-center gap-2">
                            <Handshake />
                        </span>
                        <span>Bands</span>
                    </Link>
                </div>
            </nav>
            <div className="col-start-2 col-span-3">
                <Outlet />
            </div>
        </div>
    )
}
