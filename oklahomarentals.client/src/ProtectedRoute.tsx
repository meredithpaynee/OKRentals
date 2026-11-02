import { useEffect, useRef, type ReactNode } from 'react'
import { useAuth } from 'react-oidc-context'
import SiteLayout from './site/SiteLayout'

// Remember the current page so main.tsx can return here after Keycloak.
const signIn = (auth: ReturnType<typeof useAuth>) =>
    auth.signinRedirect({ state: { returnTo: window.location.pathname } })

export default function ProtectedRoute({ children }: { children: ReactNode }) {
    const auth = useAuth()
    const needsLogin = !auth.isLoading && !auth.isAuthenticated && !auth.activeNavigator && !auth.error

    // Guard so React's development double-run of effects doesn't redirect twice.
    const redirecting = useRef(false)

    useEffect(() => {
        if (!needsLogin || redirecting.current) return
        redirecting.current = true
        signIn(auth).catch(err => {
            redirecting.current = false
            console.error('Sign-in redirect failed', err)
        })
    }, [needsLogin, auth])

    if (auth.error) {
        console.error('Sign-in error', auth.error)
        return (
            <SiteLayout>
                <div className="container notice">
                    <h2>Sign-in isn't available right now</h2>
                    <p>We couldn't reach the sign-in service. Please try again in a moment.</p>
                    <button className="button" onClick={() => signIn(auth).catch(() => {})}>
                        Try again
                    </button>
                </div>
            </SiteLayout>
        )
    }

    if (!auth.isAuthenticated) {
        return (
            <SiteLayout>
                <div className="container notice" aria-busy="true">Redirecting to sign in…</div>
            </SiteLayout>
        )
    }

    return <>{children}</>
}
