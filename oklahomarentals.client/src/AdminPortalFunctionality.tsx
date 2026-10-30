import { useState, type FormEvent } from 'react'
import { authFetch } from './auth'
import { Field, FormStatus, type SubmitStatus } from './portal/PortalLayout'
import type { PropertyResponse, TenantResponse } from './portal/types'

// Forms behind the admin portal's action buttons. Each calls onDone after a
// successful change so the portal can reload its tables.

interface FormProps {
    onDone: () => void
}

// Sends a JSON request and turns the outcome into a status line for the form.
async function submit(url: string, method: string, body: unknown, success: string): Promise<SubmitStatus> {
    try {
        const response = await authFetch(url, {
            method,
            headers: body === undefined ? undefined : { 'Content-Type': 'application/json' },
            body: body === undefined ? undefined : JSON.stringify(body),
        })
        if (response.ok) return { kind: 'ok', text: success }
        const errorData = await response.json().catch(() => ({}))
        return { kind: 'error', text: `Error ${response.status}: ${errorData.message || response.statusText || 'request failed'}` }
    } catch (error) {
        console.error(`${method} ${url} failed`, error)
        return { kind: 'error', text: 'Couldn’t reach the server. Please try again.' }
    }
}

function useSubmit(onDone: () => void) {
    const [status, setStatus] = useState<SubmitStatus>(null)
    const run = async (url: string, method: string, body: unknown, success: string) => {
        setStatus({ kind: 'busy', text: 'Saving…' })
        const result = await submit(url, method, body, success)
        setStatus(result)
        if (result?.kind === 'ok') onDone()
        return result?.kind === 'ok'
    }
    return { status, run, busy: status?.kind === 'busy' }
}

/********************************************************************************************************************
 * Create New Property
 *******************************************************************************************************************/

export function CreateProperty({ onDone }: FormProps) {
    const [name, setName] = useState('')
    const [address, setAddress] = useState('')
    const [campus, setCampus] = useState('')
    const [bedrooms, setBedrooms] = useState('')
    const [bathrooms, setBathrooms] = useState('')
    const [price, setPrice] = useState('')
    const [nextAvailable, setNextAvailable] = useState('')
    const [description, setDescription] = useState('')
    // TODO: remove once the signed-in admin is linked to an admin record.
    const [adminId, setAdminId] = useState('1')
    const { status, run, busy } = useSubmit(onDone)

    async function handleSubmit(e: FormEvent) {
        e.preventDefault()
        const ok = await run('/api/properties', 'POST', {
            Name: name,
            Campus: campus,
            Bedrooms: Number(bedrooms),
            Bathrooms: Number(bathrooms),
            Price: Number(price),
            Address: address,
            Description: description,
            AdminId: Number(adminId),
            NextAvailable: nextAvailable || undefined,
        }, `Added ${name}.`)
        if (ok) {
            setName(''); setAddress(''); setCampus(''); setBedrooms(''); setBathrooms('')
            setPrice(''); setNextAvailable(''); setDescription('')
        }
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-grid">
                <Field label="Property name" htmlFor="prop-name" full>
                    <input id="prop-name" value={name} onChange={e => setName(e.target.value)} maxLength={200} required />
                </Field>
                <Field label="Full address" htmlFor="prop-address" full>
                    <input id="prop-address" value={address} onChange={e => setAddress(e.target.value)} maxLength={500} required />
                </Field>
                <Field label="Campus" htmlFor="prop-campus">
                    <select id="prop-campus" value={campus} onChange={e => setCampus(e.target.value)} required>
                        <option value="" disabled>Select a campus</option>
                        <option>Norman</option>
                        <option>Stillwater</option>
                    </select>
                </Field>
                <Field label="Monthly rent ($)" htmlFor="prop-price">
                    <input id="prop-price" type="number" min="0" step="0.01" value={price} onChange={e => setPrice(e.target.value)} required />
                </Field>
                <Field label="Bedrooms" htmlFor="prop-bedrooms">
                    <input id="prop-bedrooms" type="number" min="0" max="10" value={bedrooms} onChange={e => setBedrooms(e.target.value)} required />
                </Field>
                <Field label="Bathrooms" htmlFor="prop-bathrooms">
                    <input id="prop-bathrooms" type="number" min="0" max="10" value={bathrooms} onChange={e => setBathrooms(e.target.value)} required />
                </Field>
                <Field label="Next available" htmlFor="prop-available">
                    <input id="prop-available" type="date" value={nextAvailable} onChange={e => setNextAvailable(e.target.value)} />
                </Field>
                <Field label="Admin ID" htmlFor="prop-admin">
                    <input id="prop-admin" type="number" min="1" value={adminId} onChange={e => setAdminId(e.target.value)} required />
                </Field>
                <Field label="Description" htmlFor="prop-description" full>
                    <textarea id="prop-description" value={description} onChange={e => setDescription(e.target.value)} maxLength={2000} />
                </Field>
            </div>
            <div className="form-actions">
                <button type="submit" className="button" disabled={busy}>Add property</button>
            </div>
            <FormStatus status={status} />
        </form>
    )
}

/********************************************************************************************************************
 * Delete Property
 *******************************************************************************************************************/

export function RemoveProperty({ properties, onDone }: FormProps & { properties: PropertyResponse[] }) {
    const [selectedId, setSelectedId] = useState('')
    const { status, run, busy } = useSubmit(onDone)
    const selected = properties.find(p => String(p.id) === selectedId)

    async function handleSubmit(e: FormEvent) {
        e.preventDefault()
        if (!selected) return
        if (await run(`/api/properties/${selected.id}`, 'DELETE', undefined, `Removed ${selected.name}.`)) setSelectedId('')
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-grid">
                <Field label="Property" htmlFor="remove-property" full>
                    <select id="remove-property" value={selectedId} onChange={e => setSelectedId(e.target.value)} required>
                        <option value="" disabled>Select a property</option>
                        {properties.map(p => <option key={p.id} value={p.id}>{p.name} — {p.address}</option>)}
                    </select>
                </Field>
            </div>
            <p className="form-note">This permanently deletes the property listing.</p>
            <div className="form-actions">
                <button type="submit" className="button button--danger" disabled={busy || !selected}>Remove property</button>
            </div>
            <FormStatus status={status} />
        </form>
    )
}

/********************************************************************************************************************
 * Create Tenant
 *******************************************************************************************************************/

export function CreateTenant({ properties, onDone }: FormProps & { properties: PropertyResponse[] }) {
    const [propertyId, setPropertyId] = useState('')
    const [name, setName] = useState('')
    const [email, setEmail] = useState('')
    const [phoneNumber, setPhoneNumber] = useState('')
    const [birthDate, setBirthDate] = useState('')
    const { status, run, busy } = useSubmit(onDone)

    async function handleSubmit(e: FormEvent) {
        e.preventDefault()
        const ok = await run('/api/tenants', 'POST', {
            propertyId: Number(propertyId),
            name,
            email,
            phoneNumber,
            birthDate,
            // UserId is auto-created by the backend
        }, `Added ${name}.`)
        if (ok) {
            setName(''); setEmail(''); setPhoneNumber(''); setBirthDate('')
        }
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-grid">
                <Field label="Property" htmlFor="tenant-property" full>
                    <select id="tenant-property" value={propertyId} onChange={e => setPropertyId(e.target.value)} required>
                        <option value="" disabled>Select a property</option>
                        {properties.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
                    </select>
                </Field>
                <Field label="Full name" htmlFor="tenant-name" full>
                    <input id="tenant-name" value={name} onChange={e => setName(e.target.value)} required />
                </Field>
                <Field label="Email" htmlFor="tenant-email">
                    <input id="tenant-email" type="email" value={email} onChange={e => setEmail(e.target.value)} required />
                </Field>
                <Field label="Phone" htmlFor="tenant-phone">
                    <input id="tenant-phone" type="tel" value={phoneNumber} onChange={e => setPhoneNumber(e.target.value)} />
                </Field>
                <Field label="Date of birth" htmlFor="tenant-dob">
                    <input id="tenant-dob" type="date" value={birthDate} onChange={e => setBirthDate(e.target.value)} />
                </Field>
            </div>
            <div className="form-actions">
                <button type="submit" className="button" disabled={busy}>Add tenant</button>
            </div>
            <FormStatus status={status} />
        </form>
    )
}

/********************************************************************************************************************
 * Remove Tenant
 *******************************************************************************************************************/

export function RemoveTenant({ tenants, onDone }: FormProps & { tenants: TenantResponse[] }) {
    const [selectedId, setSelectedId] = useState('')
    const { status, run, busy } = useSubmit(onDone)
    const selected = tenants.find(t => String(t.id) === selectedId)

    async function handleSubmit(e: FormEvent) {
        e.preventDefault()
        if (!selected) return
        if (await run(`/api/tenants/${selected.id}`, 'DELETE', undefined, `Removed ${selected.name}.`)) setSelectedId('')
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-grid">
                <Field label="Tenant" htmlFor="remove-tenant" full>
                    <select id="remove-tenant" value={selectedId} onChange={e => setSelectedId(e.target.value)} required>
                        <option value="" disabled>Select a tenant</option>
                        {tenants.map(t => <option key={t.id} value={t.id}>{t.name} — {t.email}</option>)}
                    </select>
                </Field>
            </div>
            <div className="form-actions">
                <button type="submit" className="button button--danger" disabled={busy || !selected}>Remove tenant</button>
            </div>
            <FormStatus status={status} />
        </form>
    )
}

/********************************************************************************************************************
 * Set a Property to Leased / Available
 *******************************************************************************************************************/

export function SetPropertyToLeased({ properties, onDone }: FormProps & { properties: PropertyResponse[] }) {
    const [propertyId, setPropertyId] = useState('')
    const [isLeased, setIsLeased] = useState(true)
    const { status, run, busy } = useSubmit(onDone)

    const chooseProperty = (id: string) => {
        setPropertyId(id)
        const property = properties.find(p => String(p.id) === id)
        if (property) setIsLeased(!property.isLeased)
    }

    async function handleSubmit(e: FormEvent) {
        e.preventDefault()
        const name = properties.find(p => String(p.id) === propertyId)?.name ?? 'Property'
        await run(`/api/properties/${propertyId}/availability`, 'PATCH', { isLeased },
            `${name} is now marked ${isLeased ? 'leased' : 'available'}.`)
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-grid">
                <Field label="Property" htmlFor="avail-property" full>
                    <select id="avail-property" value={propertyId} onChange={e => chooseProperty(e.target.value)} required>
                        <option value="" disabled>Select a property</option>
                        {properties.map(p => (
                            <option key={p.id} value={p.id}>{p.name} ({p.isLeased ? 'leased' : 'available'})</option>
                        ))}
                    </select>
                </Field>
                <Field label="Status" htmlFor="avail-status">
                    <select id="avail-status" value={isLeased ? 'leased' : 'available'} onChange={e => setIsLeased(e.target.value === 'leased')}>
                        <option value="available">Available</option>
                        <option value="leased">Leased</option>
                    </select>
                </Field>
            </div>
            <div className="form-actions">
                <button type="submit" className="button" disabled={busy}>Save</button>
            </div>
            <FormStatus status={status} />
        </form>
    )
}

/********************************************************************************************************************
 * Record a Payment from a Tenant
 *******************************************************************************************************************/

export function RecordPayment({ properties, tenants, onDone }: FormProps & { properties: PropertyResponse[]; tenants: TenantResponse[] }) {
    const [propertyId, setPropertyId] = useState(properties[0] ? String(properties[0].id) : '')
    const [tenantId, setTenantId] = useState('')
    const [dueDate, setDueDate] = useState('')
    const [amountDue, setAmountDue] = useState('')
    const [amountPaid, setAmountPaid] = useState('')
    const { status, run, busy } = useSubmit(onDone)

    const propertyTenants = tenants.filter(t => String(t.propertyId) === propertyId)

    async function handleSubmit(e: FormEvent) {
        e.preventDefault()
        const ok = await run('/api/payments', 'POST', {
            tenantId: Number(tenantId),
            propertyId: Number(propertyId),
            dueDate,
            paymentDate: new Date().toISOString(),
            amountDue: parseFloat(amountDue) || 0,
            amountPaid: parseFloat(amountPaid) || 0,
            status: 'Paid',
        }, 'Payment recorded.')
        if (ok) {
            setDueDate(''); setAmountDue(''); setAmountPaid('')
        }
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-grid">
                <Field label="Property" htmlFor="pay-property">
                    <select id="pay-property" value={propertyId} onChange={e => { setPropertyId(e.target.value); setTenantId('') }} required>
                        {properties.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
                    </select>
                </Field>
                <Field label="Tenant" htmlFor="pay-tenant">
                    <select id="pay-tenant" value={tenantId} onChange={e => setTenantId(e.target.value)} required>
                        <option value="" disabled>
                            {propertyTenants.length ? 'Select a tenant' : 'No tenants at this property'}
                        </option>
                        {propertyTenants.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
                    </select>
                </Field>
                <Field label="Due date" htmlFor="pay-due">
                    <input id="pay-due" type="date" value={dueDate} onChange={e => setDueDate(e.target.value)} required />
                </Field>
                <Field label="Amount due ($)" htmlFor="pay-amount-due">
                    <input id="pay-amount-due" type="number" min="0" step="0.01" value={amountDue} onChange={e => setAmountDue(e.target.value)} required />
                </Field>
                <Field label="Amount paid ($)" htmlFor="pay-amount-paid">
                    <input id="pay-amount-paid" type="number" min="0" step="0.01" value={amountPaid} onChange={e => setAmountPaid(e.target.value)} required />
                </Field>
            </div>
            <div className="form-actions">
                <button type="submit" className="button" disabled={busy}>Record payment</button>
            </div>
            <FormStatus status={status} />
        </form>
    )
}
