import { useEffect, useState, type FormEvent } from 'react'
import { useAuth } from 'react-oidc-context'
import { authFetch } from './auth'
import PortalLayout, { Badge, Dialog, Empty, Field, FormStatus, LoadError, Panel, Stat, type SubmitStatus } from './portal/PortalLayout'
import { formatDate, formatMoney, isOpenRequest } from './portal/format'

// TODO: these should come from the signed-in user's tenant record once
// Keycloak users are linked to tenants.
const TENANT_ID = 123
const PROPERTY_ID = 1

interface MaintenanceRequestResponse {
    id: number
    propertyId: number
    title: string
    description: string
    status: string
    dateRequested: string
    dateResolved: string | null
}

interface PaymentResponse {
    id: number
    tenantId: number
    propertyId: number
    dueDate: string
    paymentDate: string | null
    amountDue: number
    amountPaid: number
    status: string
}

interface Props {
    access: string
    address: string
}

export default function PortalHome({ address }: Props) {
    const auth = useAuth()
    const firstName = auth.user?.profile.given_name

    const [payments, setPayments] = useState<PaymentResponse[] | null>(null)
    const [paymentsError, setPaymentsError] = useState<string | null>(null)
    const [requests, setRequests] = useState<MaintenanceRequestResponse[] | null>(null)
    const [requestsError, setRequestsError] = useState<string | null>(null)
    const [showRequestForm, setShowRequestForm] = useState(false)

    useEffect(() => {
        authFetch(`/api/payments?TenantID=${TENANT_ID}`)
            .then(res => { if (!res.ok) throw new Error(`HTTP ${res.status}`); return res.json() })
            .then((data: PaymentResponse[]) => setPayments(data.filter(p => p.tenantId === TENANT_ID)))
            .catch(err => setPaymentsError(err.message))
    }, [])

    const loadRequests = () => {
        authFetch(`/api/maintenance-requests?propertyId=${PROPERTY_ID}`)
            .then(res => { if (!res.ok) throw new Error(`HTTP ${res.status}`); return res.json() })
            .then((data: MaintenanceRequestResponse[]) => setRequests(data.filter(r => r.propertyId === PROPERTY_ID)))
            .catch(err => setRequestsError(err.message))
    }
    useEffect(loadRequests, [])

    const balance = payments?.reduce((sum, p) => sum + (p.amountDue - p.amountPaid), 0) ?? null
    const nextDue = payments
        ?.filter(p => p.amountPaid < p.amountDue)
        .sort((a, b) => a.dueDate.localeCompare(b.dueDate))[0]
    const openRequests = requests?.filter(r => isOpenRequest(r.status)).length ?? null

    return (
        <PortalLayout
            title={firstName ? `Welcome back, ${firstName}` : 'Resident portal'}
            subtitle={address || undefined}
            sections={[
                { id: 'payments', label: 'Payments' },
                { id: 'maintenance', label: 'Maintenance' },
                { id: 'documents', label: 'Documents' },
            ]}
        >
            <div className="stats">
                <Stat
                    label="Current balance"
                    value={balance === null ? '—' : formatMoney(Math.max(balance, 0))}
                    note={balance !== null && balance <= 0 ? 'You’re all paid up' : undefined}
                />
                <Stat
                    label="Next payment due"
                    value={nextDue ? formatDate(nextDue.dueDate) : '—'}
                    note={nextDue ? formatMoney(nextDue.amountDue - nextDue.amountPaid) : 'Nothing due'}
                />
                <Stat label="Open maintenance requests" value={openRequests ?? '—'} />
            </div>

            <Panel id="payments" title="Payment history">
                {paymentsError ? <LoadError message={paymentsError} />
                    : payments === null ? <Empty>Loading…</Empty>
                    : payments.length === 0 ? <Empty>No payments yet.</Empty>
                    : (
                        <div className="table-wrap">
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>Due date</th>
                                        <th className="num">Amount due</th>
                                        <th className="num">Paid</th>
                                        <th>Paid on</th>
                                        <th>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {[...payments].sort((a, b) => b.dueDate.localeCompare(a.dueDate)).map(p => (
                                        <tr key={p.id}>
                                            <td className="nowrap">{formatDate(p.dueDate)}</td>
                                            <td className="num">{formatMoney(p.amountDue)}</td>
                                            <td className="num">{formatMoney(p.amountPaid)}</td>
                                            <td className="nowrap">{formatDate(p.paymentDate)}</td>
                                            <td><Badge>{p.status}</Badge></td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
            </Panel>

            <Panel
                id="maintenance"
                title="Maintenance requests"
                actions={<button className="button button--small" onClick={() => setShowRequestForm(true)}>New request</button>}
            >
                {requestsError ? <LoadError message={requestsError} />
                    : requests === null ? <Empty>Loading…</Empty>
                    : requests.length === 0 ? <Empty>You haven't submitted any requests.</Empty>
                    : (
                        <div className="table-wrap">
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>Request</th>
                                        <th>Submitted</th>
                                        <th>Resolved</th>
                                        <th>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {[...requests].sort((a, b) => b.dateRequested.localeCompare(a.dateRequested)).map(r => (
                                        <tr key={r.id}>
                                            <td>{r.title}<span className="cell-sub">{r.description}</span></td>
                                            <td className="nowrap">{formatDate(r.dateRequested)}</td>
                                            <td className="nowrap">{formatDate(r.dateResolved)}</td>
                                            <td><Badge>{r.status}</Badge></td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
            </Panel>

            <Panel id="documents" title="Documents">
                <Empty>Your lease and other documents will appear here.</Empty>
            </Panel>

            {showRequestForm && (
                <MaintenanceRequestForm
                    onClose={() => setShowRequestForm(false)}
                    onCreated={loadRequests}
                />
            )}
        </PortalLayout>
    )
}

function MaintenanceRequestForm({ onClose, onCreated }: { onClose: () => void; onCreated: () => void }) {
    const [title, setTitle] = useState('')
    const [description, setDescription] = useState('')
    const [priority, setPriority] = useState('Medium')
    const [status, setStatus] = useState<SubmitStatus>(null)

    async function handleSubmit(e: FormEvent) {
        e.preventDefault()
        setStatus({ kind: 'busy', text: 'Submitting…' })
        try {
            const response = await authFetch('/api/maintenance-requests', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    TenantId: TENANT_ID,
                    PropertyId: PROPERTY_ID,
                    Title: title,
                    Description: description,
                    RequestPriority: priority,
                }),
            })
            if (!response.ok) {
                setStatus({ kind: 'error', text: `Couldn't submit the request (${response.status}).` })
                return
            }
            onCreated()
            onClose()
        } catch (error) {
            console.error('Failed to create request', error)
            setStatus({ kind: 'error', text: 'Couldn’t reach the server. Please try again.' })
        }
    }

    return (
        <Dialog title="New maintenance request" onClose={onClose}>
            <form onSubmit={handleSubmit}>
                <div className="form-grid">
                    <Field label="What needs fixing?" htmlFor="request-title" full>
                        <input id="request-title" value={title} onChange={e => setTitle(e.target.value)} maxLength={100} required />
                    </Field>
                    <Field label="Details" htmlFor="request-description" full>
                        <textarea
                            id="request-description"
                            value={description}
                            onChange={e => setDescription(e.target.value)}
                            maxLength={1000}
                            placeholder="Where is it, and when did it start?"
                            required
                        />
                    </Field>
                    <Field label="Urgency" htmlFor="request-priority">
                        <select id="request-priority" value={priority} onChange={e => setPriority(e.target.value)}>
                            <option>Low</option>
                            <option>Medium</option>
                            <option>High</option>
                        </select>
                    </Field>
                </div>
                <div className="form-actions">
                    <button type="submit" className="button" disabled={status?.kind === 'busy'}>Submit request</button>
                    <button type="button" className="button button--outline" onClick={onClose}>Cancel</button>
                </div>
                <FormStatus status={status} />
            </form>
        </Dialog>
    )
}
