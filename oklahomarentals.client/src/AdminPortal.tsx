import { useCallback, useEffect, useState } from 'react'
import { authFetch } from './auth'
import PortalLayout, { Badge, Dialog, Empty, LoadError, Panel, Stat } from './portal/PortalLayout'
import { formatDate, formatMoney, isOpenRequest } from './portal/format'
import type { ApplicationResponse, MaintenanceRequestResponse, PaymentResponse, PropertyResponse, TenantResponse } from './portal/types'
import ApplicationDetails from './portal/ApplicationDetails'
import { PropertyForm, DeleteProperty, CreateTenant, RemoveTenant, RecordPayment } from './AdminPortalFunctionality'

interface AdminData {
    properties: PropertyResponse[]
    tenants: TenantResponse[]
    payments: PaymentResponse[]
    requests: MaintenanceRequestResponse[]
    applications: ApplicationResponse[]
}

type Action = 'createProperty' | 'createTenant' | 'removeTenant' | 'recordPayment'

const actionTitles: Record<Action, string> = {
    createProperty: 'Add a property',
    createTenant: 'Add a tenant',
    removeTenant: 'Remove a tenant',
    recordPayment: 'Record a payment',
}

function getJson<T>(url: string): Promise<T> {
    return authFetch(url).then(res => {
        if (!res.ok) throw new Error(`${url} returned ${res.status}`)
        return res.json()
    })
}

export default function AdminPortal() {
    const [data, setData] = useState<AdminData | null>(null)
    const [error, setError] = useState<string | null>(null)
    const [action, setAction] = useState<Action | null>(null)
    const [openApplicationId, setOpenApplicationId] = useState<number | null>(null)
    const [editingPropertyId, setEditingPropertyId] = useState<number | null>(null)
    const [deletingPropertyId, setDeletingPropertyId] = useState<number | null>(null)

    const load = useCallback(() => {
        Promise.all([
            getJson<PropertyResponse[]>('/api/properties'),
            getJson<TenantResponse[]>('/api/tenants'),
            getJson<PaymentResponse[]>('/api/payments'),
            getJson<MaintenanceRequestResponse[]>('/api/maintenance-requests'),
            getJson<ApplicationResponse[]>('/api/applications'),
        ])
            .then(([properties, tenants, payments, requests, applications]) => {
                setData({ properties, tenants, payments, requests, applications })
                setError(null)
            })
            .catch(err => setError(err.message))
    }, [])

    useEffect(load, [load])

    const markResolved = (id: number) => {
        authFetch(`/api/maintenance-requests/${id}/status`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ status: 'Completed' }),
        })
            .then(res => {
                if (!res.ok) throw new Error(`Couldn't update request (${res.status})`)
                load()
            })
            .catch(err => setError(err.message))
    }

    const propertyName = (id: number) => data?.properties.find(p => p.id === id)?.name ?? `Property ${id}`
    const tenantName = (id: number) => data?.tenants.find(t => t.id === id)?.name ?? `Tenant ${id}`

    const outstanding = data?.payments.reduce((sum, p) => sum + Math.max(p.amountDue - p.amountPaid, 0), 0) ?? 0
    const openRequests = data?.requests.filter(r => isOpenRequest(r.status)) ?? []
    const leased = data?.properties.filter(p => p.isLeased).length ?? 0
    const pendingApplications = data?.applications.filter(a => a.status === 'Pending').length ?? 0
    const openApplication = data?.applications.find(a => a.id === openApplicationId)
    const editingProperty = data?.properties.find(p => p.id === editingPropertyId)
    const deletingProperty = data?.properties.find(p => p.id === deletingPropertyId)

    return (
        <PortalLayout
            title="Admin"
            subtitle="Properties, tenants, payments, and maintenance."
            sections={[
                { id: 'applications', label: 'Applications' },
                { id: 'properties', label: 'Properties' },
                { id: 'payments', label: 'Payments' },
                { id: 'maintenance', label: 'Maintenance' },
                { id: 'tenants', label: 'Tenants' },
                { id: 'documents', label: 'Documents' },
            ]}
        >
            {error && <LoadError message={error} />}

            {!data && !error && <p className="portal-loading">Loading…</p>}

            {data && (
                <>
                    <div className="stats">
                        <Stat label="New applications" value={pendingApplications} note="Awaiting review" />
                        <Stat label="Properties" value={data.properties.length} note={`${leased} leased`} />
                        <Stat label="Tenants" value={data.tenants.length} />
                        <Stat label="Outstanding rent" value={formatMoney(outstanding)} />
                        <Stat label="Open maintenance" value={openRequests.length} />
                    </div>

                    <Panel id="applications" title="Rental applications">
                        {data.applications.length === 0 ? <Empty>No applications yet. Ones submitted on the Apply page will show up here.</Empty> : (
                            <div className="table-wrap">
                                <table className="data-table">
                                    <thead>
                                        <tr><th>Applicant</th><th>Property</th><th>Move-in</th><th>Submitted</th><th>Status</th><th><span className="visually-hidden">Actions</span></th></tr>
                                    </thead>
                                    <tbody>
                                        {data.applications.map(a => (
                                            <tr key={a.id}>
                                                <td>{a.name}<span className="cell-sub">{a.email}</span></td>
                                                <td>{a.propertyName ?? propertyName(a.propertyId)}</td>
                                                <td className="nowrap">{formatDate(a.moveInDate)}</td>
                                                <td className="nowrap">{formatDate(a.dateSubmitted)}</td>
                                                <td><Badge>{a.status}</Badge></td>
                                                <td className="num">
                                                    <button className="button button--small button--outline" onClick={() => setOpenApplicationId(a.id)}>
                                                        View
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </Panel>

                    <Panel
                        id="properties"
                        title="Properties"
                        actions={<button className="button button--small" onClick={() => setAction('createProperty')}>Add property</button>}
                    >
                        {data.properties.length === 0 ? <Empty>No properties yet.</Empty> : (
                            <div className="table-wrap">
                                <table className="data-table">
                                    <thead>
                                        <tr><th>Property</th><th>Campus</th><th>Beds / baths</th><th className="num">Rent</th><th>Tenants</th><th>Status</th><th><span className="visually-hidden">Actions</span></th></tr>
                                    </thead>
                                    <tbody>
                                        {data.properties.map(p => (
                                            <tr key={p.id}>
                                                <td>{p.name}<span className="cell-sub">{p.address}</span></td>
                                                <td>{p.campus}</td>
                                                <td className="nowrap">{p.bedrooms} bd · {p.bathrooms} ba</td>
                                                <td className="num">{formatMoney(p.price)}</td>
                                                <td>{data.tenants.filter(t => t.propertyId === p.id).length}</td>
                                                <td><Badge>{p.isLeased ? 'Leased' : 'Available'}</Badge></td>
                                                <td className="row-actions">
                                                    <button className="button button--small button--outline" onClick={() => setEditingPropertyId(p.id)}>
                                                        Update
                                                    </button>
                                                    <button
                                                        className="icon-button icon-button--danger"
                                                        onClick={() => setDeletingPropertyId(p.id)}
                                                        aria-label={`Delete ${p.name}`}
                                                        title={`Delete ${p.name}`}
                                                    >
                                                        <TrashIcon />
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </Panel>

                    <Panel
                        id="payments"
                        title="Payments"
                        actions={<button className="button button--small" onClick={() => setAction('recordPayment')}>Record payment</button>}
                    >
                        {data.payments.length === 0 ? <Empty>No payments recorded.</Empty> : (
                            <div className="table-wrap">
                                <table className="data-table">
                                    <thead>
                                        <tr><th>Tenant</th><th>Due</th><th className="num">Amount due</th><th className="num">Paid</th><th className="num">Owed</th><th>Status</th></tr>
                                    </thead>
                                    <tbody>
                                        {[...data.payments].sort((a, b) => b.dueDate.localeCompare(a.dueDate)).map(p => {
                                            const owed = Math.max(p.amountDue - p.amountPaid, 0)
                                            return (
                                                <tr key={p.id}>
                                                    <td>{tenantName(p.tenantId)}<span className="cell-sub">{propertyName(p.propertyId)}</span></td>
                                                    <td className="nowrap">{formatDate(p.dueDate)}</td>
                                                    <td className="num">{formatMoney(p.amountDue)}</td>
                                                    <td className="num">{formatMoney(p.amountPaid)}<span className="cell-sub">{formatDate(p.paymentDate)}</span></td>
                                                    <td className={`num${owed > 0 ? ' amount-owed' : ''}`}>{formatMoney(owed)}</td>
                                                    <td><Badge>{p.status}</Badge></td>
                                                </tr>
                                            )
                                        })}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </Panel>

                    <Panel id="maintenance" title="Maintenance requests">
                        {data.requests.length === 0 ? <Empty>No maintenance requests.</Empty> : (
                            <div className="table-wrap">
                                <table className="data-table">
                                    <thead>
                                        <tr><th>Request</th><th>Property</th><th>Submitted</th><th>Status</th><th><span className="visually-hidden">Actions</span></th></tr>
                                    </thead>
                                    <tbody>
                                        {[...data.requests]
                                            .sort((a, b) => Number(isOpenRequest(b.status)) - Number(isOpenRequest(a.status)) || b.dateRequested.localeCompare(a.dateRequested))
                                            .map(r => (
                                                <tr key={r.id}>
                                                    <td>{r.title}<span className="cell-sub">{r.description}</span></td>
                                                    <td>{propertyName(r.propertyId)}</td>
                                                    <td className="nowrap">{formatDate(r.dateRequested)}</td>
                                                    <td><Badge>{r.status}</Badge></td>
                                                    <td className="num">
                                                        {isOpenRequest(r.status) && (
                                                            <button className="button button--small button--outline" onClick={() => markResolved(r.id)}>
                                                                Mark resolved
                                                            </button>
                                                        )}
                                                    </td>
                                                </tr>
                                            ))}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </Panel>

                    <Panel
                        id="tenants"
                        title="Tenants"
                        actions={<>
                            <button className="button button--small" onClick={() => setAction('createTenant')}>Add tenant</button>
                            <button className="button button--small button--outline" onClick={() => setAction('removeTenant')}>Remove</button>
                        </>}
                    >
                        {data.tenants.length === 0 ? <Empty>No tenants yet.</Empty> : (
                            <div className="table-wrap">
                                <table className="data-table">
                                    <thead>
                                        <tr><th>Name</th><th>Property</th><th>Email</th><th>Phone</th></tr>
                                    </thead>
                                    <tbody>
                                        {[...data.tenants].sort((a, b) => a.name.localeCompare(b.name)).map(t => (
                                            <tr key={t.id}>
                                                <td>{t.name}</td>
                                                <td>{propertyName(t.propertyId)}</td>
                                                <td><a href={`mailto:${t.email}`}>{t.email}</a></td>
                                                <td className="nowrap">{t.phoneNumber}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </Panel>

                    <Panel id="documents" title="Documents">
                        <Empty>Lease documents will appear here.</Empty>
                    </Panel>
                </>
            )}

            {editingProperty && (
                <Dialog title={`Update ${editingProperty.name}`} onClose={() => setEditingPropertyId(null)}>
                    <PropertyForm property={editingProperty} onDone={load} />
                </Dialog>
            )}

            {deletingProperty && (
                <Dialog title="Delete property?" onClose={() => setDeletingPropertyId(null)}>
                    <DeleteProperty property={deletingProperty} onDone={load} onCancel={() => setDeletingPropertyId(null)} />
                </Dialog>
            )}

            {openApplication && (
                <Dialog title={`Application from ${openApplication.name}`} onClose={() => setOpenApplicationId(null)}>
                    <ApplicationDetails application={openApplication} onStatusChanged={load} />
                </Dialog>
            )}

            {action && data && (
                <Dialog title={actionTitles[action]} onClose={() => setAction(null)}>
                    {action === 'createProperty' && <PropertyForm onDone={load} />}
                    {action === 'createTenant' && <CreateTenant properties={data.properties} onDone={load} />}
                    {action === 'removeTenant' && <RemoveTenant tenants={data.tenants} onDone={load} />}
                    {action === 'recordPayment' && <RecordPayment properties={data.properties} tenants={data.tenants} onDone={load} />}
                </Dialog>
            )}
        </PortalLayout>
    )
}

function TrashIcon() {
    return (
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
            <path d="M3 6h18" />
            <path d="M8 6V4h8v2" />
            <path d="M19 6l-1 14H6L5 6" />
            <path d="M10 11v6M14 11v6" />
        </svg>
    )
}
