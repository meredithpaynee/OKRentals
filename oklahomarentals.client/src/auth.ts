import { User } from 'oidc-client-ts'

const authority = 'http://localhost:8080/realms/oklahoma-rentals'
const clientId = 'oklahoma-rentals-frontend'

export const oidcConfig = {
    authority,
    client_id: clientId,
    redirect_uri: `${window.location.origin}/portal`,
    post_logout_redirect_uri: window.location.origin,
    scope: 'openid profile email',
}

// fetch() that sends the signed-in user's access token, for [Authorize] API endpoints.
export function authFetch(input: RequestInfo | URL, init: RequestInit = {}): Promise<Response> {
    const stored = sessionStorage.getItem(`oidc.user:${authority}:${clientId}`)
    const token = stored ? User.fromStorageString(stored).access_token : null

    const headers = new Headers(init.headers)
    if (token) headers.set('Authorization', `Bearer ${token}`)
    return fetch(input, { ...init, headers })
}
