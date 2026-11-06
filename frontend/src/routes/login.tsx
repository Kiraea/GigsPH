import { useState } from 'react'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/login')({
    component: Login,
})

function Login() {
    const submitLogin = (e) => {
        e.preventDefault()
        const form = new FormData(e.currentTarget)
        console.log(form.get('username'))
        console.log(form.get('password'))
    }
    console.log(import.meta.env.VITE_API_URL)

    return (
        <div className="bg-gray-500 min-h-screen flex items-center justify-center border-2px">
            <div className="flex w-[50%] ">
                <div className="bg-green-300 flex-1"></div>

                <form
                    className="bg-white flex-1 flex-col"
                    onSubmit={submitLogin}
                >
                    <div className="flex flex-col">
                        <div>Username</div>
                        <input type="text" name="username" />
                    </div>
                    <div className="flex flex-col">
                        <div>Password</div>
                        <input type="password" name="password" />
                    </div>
                    <button type="submit">Login</button>
                </form>
            </div>
        </div>
    )
}
