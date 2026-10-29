import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App'
import { AuthProvider } from 'react-oidc-context'
import { oidcConfig } from './auth'
import './index.css'

// After Keycloak redirects back, drop ?code=&state= from the address bar.
const onSigninCallback = () => {
    window.history.replaceState({}, document.title, window.location.pathname)
}

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <AuthProvider {...oidcConfig} onSigninCallback={onSigninCallback}>
            <App />
        </AuthProvider>
    </StrictMode>
)
