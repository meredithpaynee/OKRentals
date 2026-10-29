import React, { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App'
import { AuthProvider } from 'react-oidc-context'
import './index.css'

const oidcConfig = {
    authority: 'http://localhost:8080/realms/oklahoma-rentals',
    client_id: 'oklahoma-rentals-frontend',
    redirect_uri: 'http://localhost:52106/portal',
    post_logout_redirect_uri: 'http://localhost:52106',
    scope: 'openid profile email',
}

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <AuthProvider {...oidcConfig}>
            <App />
        </AuthProvider>
    </StrictMode>
)