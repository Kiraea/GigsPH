import { createFileRoute, Link } from '@tanstack/react-router'
import { type LoginRequest, useLogin } from '#/queries/auth/api.ts'
import type { SubmitEvent } from 'react'
export const Route = createFileRoute('/login')({
    component: LoginPage,
})

function LoginPage() {
    const submitLogin = (e: SubmitEvent<HTMLFormElement>) => {
        e.preventDefault()
        const form = new FormData(e.currentTarget)
        const usernameOrEmail = form.get('usernameOrEmail')
        const password = form.get('password')
        if (
            typeof usernameOrEmail !== 'string' ||
            typeof password !== 'string'
        ) {
            return
        }
        const request: LoginRequest = {
            usernameOrEmail: usernameOrEmail,
            password: password,
        }
        const login = useLogin()
        login.mutate(request)
    }
    console.log(import.meta.env.VITE_API_URL)

    return (
        <div className=" min-h-screen flex items-center justify-center border-2px ">
            <div className="grid w-full aspect-[3/1] max-w-4xl grid-cols-2 rounded-2xl overflow-hidden shadow-md">
                <div className="border-black flex flex-col bg-emerald-400 flex items-center justify-center">
                    <div className="font-extrabold text-white text-4xl">
                        SessionPH
                    </div>
                    <div className="text-white font-semibold text-md">
                        Login
                    </div>
                </div>

                <form
                    className="bg-periwinkle  flex-col flex justify-center p-5 "
                    onSubmit={submitLogin}
                >
                    <div className="flex flex-col gap-5">
                        <div className="flex flex-col">
                            <div className="">Username</div>
                            <input type="text" name="usernameOrEmail" />
                        </div>
                        <div className="flex flex-col">
                            <div className="">Password</div>
                            <input type="password" name="password" />
                        </div>
                        <div className="flex gap-5 justify-between">
                            <div className="">
                                Don't have an Account yet?{' '}
                                <Link to="/register">
                                    <span className="text-emerald-500">
                                        Sign In
                                    </span>
                                </Link>
                            </div>
                            <button
                                type="submit"
                                className=" bg-emerald-500 px-4 py-2 whitespace-nowrap font-bold rounded-xl text-white"
                            >
                                Login
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    )
}
