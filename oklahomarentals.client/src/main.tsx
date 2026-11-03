import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App'
import { AuthProvider } from 'react-oidc-context'
import type { User } from 'oidc-client-ts'
import { oidcConfig } from './auth'
import './index.css'

// Keycloak always redirects back to /portal. If sign-in started on another
// page (e.g. /portal/admin), go back there; otherwise just drop ?code=&state=.
const onSigninCallback = (user: User | undefined) => {
    const returnTo = (user?.state as { returnTo?: string } | undefined)?.returnTo
    if (returnTo && returnTo.startsWith('/') && returnTo !== window.location.pathname) {
        window.location.replace(returnTo)
    } else {
        window.history.replaceState({}, document.title, window.location.pathname)
    }
}

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <AuthProvider {...oidcConfig} onSigninCallback={onSigninCallback}>
            <App />
        </AuthProvider>
    </StrictMode>
)
