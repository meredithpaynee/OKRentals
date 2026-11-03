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

// Whether the user has Keycloak's realm "admin" role. Only decides what the UI
// shows; the API enforces the role on every admin endpoint itself.
export function isAdmin(user: User | null | undefined): boolean {
    const payload = user?.access_token?.split('.')[1]
    if (!payload) return false
    try {
        const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
        const roles: unknown = JSON.parse(json)?.realm_access?.roles
        return Array.isArray(roles) && roles.includes('admin')
    } catch {
        return false
    }
}
