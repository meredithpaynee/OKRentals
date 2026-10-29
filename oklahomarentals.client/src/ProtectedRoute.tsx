import { useEffect, type ReactNode } from 'react'
import { useAuth } from 'react-oidc-context'

export default function ProtectedRoute({ children }: { children: ReactNode }) {
    const auth = useAuth()
    const needsLogin = !auth.isLoading && !auth.isAuthenticated && !auth.activeNavigator && !auth.error

    useEffect(() => {
        if (needsLogin) auth.signinRedirect()
    }, [needsLogin, auth])

    if (auth.error) {
        return <div>Sign-in failed: {auth.error.message}</div>
    }

    if (!auth.isAuthenticated) {
        return <div>Loading...</div>
    }

    return <>{children}</>
}
