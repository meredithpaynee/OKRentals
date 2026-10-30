import { useEffect, useRef, type ReactNode } from 'react'
import { useAuth } from 'react-oidc-context'
import Header from '../Header'
import '../site/site.css'
import './portal.css'

interface PortalLayoutProps {
    title: string
    subtitle?: string
    sections: { id: string; label: string }[]
    children: ReactNode
}

// Shell for the resident and admin portals: site header, section nav, account box.
export default function PortalLayout({ title, subtitle, sections, children }: PortalLayoutProps) {
    const auth = useAuth()
    const profile = auth.user?.profile
    const displayName = profile?.name || profile?.preferred_username || 'Signed in'

    return (
        <div className="site portal">
            <Header />
            <div className="container portal__body">
                <aside className="portal__sidebar">
                    <nav aria-label="Portal sections">
                        {sections.map(s => <a key={s.id} href={`#${s.id}`}>{s.label}</a>)}
                    </nav>
                    <div className="portal__account">
                        <span className="portal__account-name">{displayName}</span>
                        {profile?.email && <span>{profile.email}</span>}
                        <button className="link-button" onClick={() => auth.signoutRedirect()}>Sign out</button>
                    </div>
                </aside>
                <main className="portal__main">
                    <div className="portal__title">
                        <h1>{title}</h1>
                        {subtitle && <p>{subtitle}</p>}
                    </div>
                    {children}
                </main>
            </div>
        </div>
    )
}

export function Stat({ label, value, note }: { label: string; value: ReactNode; note?: string }) {
    return (
        <div className="stat">
            <span className="stat__label">{label}</span>
            <span className="stat__value">{value}</span>
            {note && <span className="stat__note">{note}</span>}
        </div>
    )
}

interface PanelProps {
    id: string
    title: string
    actions?: ReactNode
    children: ReactNode
}

export function Panel({ id, title, actions, children }: PanelProps) {
    return (
        <section className="portal-panel" id={id} aria-labelledby={`${id}-title`}>
            <div className="portal-panel__head">
                <h2 id={`${id}-title`}>{title}</h2>
                {actions && <div className="portal-panel__actions">{actions}</div>}
            </div>
            {children}
        </section>
    )
}

export function Empty({ children }: { children: ReactNode }) {
    return <p className="portal-empty">{children}</p>
}

export function LoadError({ message }: { message: string }) {
    return <p className="portal-empty portal-empty--error">Couldn't load this section ({message}). Refresh the page to try again.</p>
}

const badgeTone: Record<string, string> = {
    paid: 'good', completed: 'good', resolved: 'good', available: 'good',
    pending: 'warn', partial: 'warn', leased: 'neutral',
    late: 'bad', cancelled: 'neutral',
}

export function Badge({ children }: { children: string }) {
    const tone = badgeTone[children.toLowerCase()] ?? 'neutral'
    return <span className={`badge badge--${tone}`}>{children}</span>
}

interface DialogProps {
    title: string
    onClose: () => void
    children: ReactNode
}

// Modal dialog built on <dialog>, so focus trapping and Escape come from the browser.
export function Dialog({ title, onClose, children }: DialogProps) {
    const ref = useRef<HTMLDialogElement>(null)

    useEffect(() => {
        const dialog = ref.current
        if (dialog && !dialog.open) dialog.showModal()
    }, [])

    return (
        <dialog ref={ref} className="portal-dialog" onClose={onClose} aria-labelledby="dialog-title">
            <div className="portal-dialog__head">
                <h2 id="dialog-title">{title}</h2>
                <button className="portal-dialog__close" onClick={onClose} aria-label="Close">&times;</button>
            </div>
            <div className="portal-dialog__body">{children}</div>
        </dialog>
    )
}

interface FieldProps {
    label: string
    htmlFor: string
    children: ReactNode
    full?: boolean
}

export function Field({ label, htmlFor, children, full }: FieldProps) {
    return (
        <div className={`field${full ? ' field--full' : ''}`}>
            <label htmlFor={htmlFor}>{label}</label>
            {children}
        </div>
    )
}

export type SubmitStatus = { kind: 'ok' | 'error' | 'busy'; text: string } | null

// Result line under a form after it's submitted.
export function FormStatus({ status }: { status: SubmitStatus }) {
    if (!status) return null
    return <p className={`form-result form-result--${status.kind}`} role="status">{status.text}</p>
}
