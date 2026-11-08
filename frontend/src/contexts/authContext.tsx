import {
    createContext,
    type Dispatch,
    type SetStateAction,
    useEffect,
} from 'react'
import { useState } from 'react'
import { axiosInstance } from '#/axiosInstance.tsx'
interface UserType {
    displayName: string | null
    id: string
    isOnboarded: boolean
}

interface AuthContextType {
    isLoggedIn: boolean
    setIsLoggedIn: Dispatch<SetStateAction<boolean>>
    isLoading: boolean
    setIsLoading: Dispatch<SetStateAction<boolean>>
    user: UserType | null
    setUser: Dispatch<SetStateAction<UserType | null>>
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined)

export const AuthContextProvider = ({ children }) => {
    const [isLoggedIn, setIsLoggedIn] = useState(false)
    const [isLoading, setIsLoading] = useState(true)
    const [user, setUser] = useState<UserType | null>(null)

    useEffect(() => {
        setIsLoading(true)
        const checkSessionToken = async () => {
            setIsLoading(true)
            try {
                const response = await axiosInstance.get(`/checkToken`)
                setUser({
                    displayName: response.data.displayName,
                    id: response.data.id,
                    isOnboarded: response.data.isOnboarded,
                })
                setIsLoggedIn(true)
            } catch (error) {
                setUser(null)
                setIsLoggedIn(false)
            } finally {
                setIsLoading(false)
            }
        }
        checkSessionToken()
    }, [])
    return (
        <AuthContext.Provider
            value={{
                isLoggedIn,
                setIsLoggedIn,
                isLoading,
                setIsLoading,
                user,
                setUser,
            }}
        >
            {children}
        </AuthContext.Provider>
    )
}
