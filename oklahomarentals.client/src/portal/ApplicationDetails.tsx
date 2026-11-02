import { useState, type ReactNode } from 'react'
import { authFetch } from '../auth'
import { Badge, FormStatus, type SubmitStatus } from './PortalLayout'
import { formatDate } from './format'
import type { ApplicationResponse, ResidenceResponse } from './types'

interface Props {
    application: ApplicationResponse
    onStatusChanged: () => void
}

// Everything an applicant submitted, plus approve/decline buttons. Shown in a dialog.
export default function ApplicationDetails({ application: a, onStatusChanged }: Props) {
    const [status, setStatus] = useState(a.status)
    const [result, setResult] = useState<SubmitStatus>(null)

    async function changeStatus(next: string) {
        setResult({ kind: 'busy', text: 'Saving…' })
        try {
            const res = await authFetch(`/api/applications/${a.id}/status`, {
                method: 'PATCH',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ status: next }),
            })
            if (!res.ok) {
                setResult({ kind: 'error', text: `Couldn't update the status (${res.status}).` })
                return
            }
            setStatus(next)
            setResult({ kind: 'ok', text: `Marked ${next.toLowerCase()}.` })
            onStatusChanged()
        } catch {
            setResult({ kind: 'error', text: 'Couldn’t reach the server. Please try again.' })
        }
    }

    const yesNo = (v: boolean | null) => (v === null ? null : v ? 'Yes' : 'No')

    return (
        <div className="application-details">
            <div className="application-details__status">
                <Badge>{status}</Badge>
                <span>Submitted {formatDate(a.dateSubmitted)}</span>
            </div>

            <Details title="Applicant" rows={[
                ['Name', a.name],
                ['Email', <a href={`mailto:${a.email}`}>{a.email}</a>],
                ['Phone', <a href={`tel:${a.phoneNumber}`}>{a.phoneNumber}</a>],
                ['Date of birth', formatDate(a.birthDate)],
                ["Driver's license", a.driversLicense],
            ]} />
            <Details title="Property and lease" rows={[
                ['Property', a.propertyName],
                ['Requested start', formatDate(a.moveInDate)],
                ['Lease term', a.leaseTerm],
            ]} />
            <Details title="School" rows={[
                ['College / university', a.college],
                ['Major', a.major],
                ['Expected graduation', a.expectedGraduation && formatDate(a.expectedGraduation)],
            ]} />
            <Residence title="Current residence" residence={a.currentResidence} />
            <Residence title="Previous residence" residence={a.previousResidence} />
            <Details title="Background" rows={[
                ['Ever been evicted', yesNo(a.evicted)],
                ['Ever broken a lease', yesNo(a.brokenLease)],
                ['Ever filed for bankruptcy', yesNo(a.bankruptcy)],
                ['Ever convicted of a felony or misdemeanor', yesNo(a.convicted)],
                ['Explanation', a.explanation],
            ]} />

            <div className="form-actions">
                {status !== 'Approved' && (
                    <button className="button" disabled={result?.kind === 'busy'} onClick={() => changeStatus('Approved')}>Approve</button>
                )}
                {status !== 'Declined' && (
                    <button className="button button--outline" disabled={result?.kind === 'busy'} onClick={() => changeStatus('Declined')}>Decline</button>
                )}
                {status !== 'Pending' && (
                    <button className="button button--outline" disabled={result?.kind === 'busy'} onClick={() => changeStatus('Pending')}>Move back to pending</button>
                )}
            </div>
            <FormStatus status={result} />
        </div>
    )
}

function Residence({ title, residence: r }: { title: string; residence: ResidenceResponse | null }) {
    if (!r) return null
    const dates = r.from || r.to ? `${formatDate(r.from)} – ${formatDate(r.to)}` : null
    return (
        <Details title={title} rows={[
            ['Address', r.address],
            ['Landlord / manager', r.landlord],
            ['Landlord contact', r.landlordContact],
            ['Dates', dates],
            ['Reason for leaving', r.reasonForLeaving],
        ]} />
    )
}

// A titled list of label/value pairs; empty values are left out.
function Details({ title, rows }: { title: string; rows: [string, ReactNode][] }) {
    const filled = rows.filter(([, value]) => value !== null && value !== undefined && value !== '')
    if (filled.length === 0) return null
    return (
        <section className="application-details__section">
            <h3>{title}</h3>
            <dl>
                {filled.map(([label, value]) => (
                    <div key={label}>
                        <dt>{label}</dt>
                        <dd>{value}</dd>
                    </div>
                ))}
            </dl>
        </section>
    )
}
