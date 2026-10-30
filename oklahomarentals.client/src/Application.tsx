import { useState, type FormEvent, type InputHTMLAttributes, type ReactNode } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import SiteLayout, { PageHeader } from './site/SiteLayout'
import { CAMPUSES, CONTACT_EMAIL, CONTACT_PHONE } from './site/siteInfo'
import { useProperties } from './site/api'

type SubmitState = 'idle' | 'sending' | 'sent' | 'error'

// Turns the form fields (named by their ids) into the API's RentalApplicationRequest.
function toRequest(form: HTMLFormElement) {
    const data = new FormData(form)
    const value = (name: string) => {
        const v = data.get(name)
        return typeof v === 'string' && v.trim() !== '' ? v.trim() : null
    }
    const residence = (prefix: string) => ({
        address: value(`${prefix}-address`),
        landlord: value(`${prefix}-landlord`),
        landlordContact: value(`${prefix}-landlord-contact`),
        from: value(`${prefix}-from`),
        to: value(`${prefix}-to`),
        reasonForLeaving: value(`${prefix}-reason`),
    })

    return {
        name: value('applicant-name'),
        email: value('applicant-email'),
        phone: value('applicant-phone'),
        dateOfBirth: value('applicant-dob'),
        driversLicense: value('applicant-license'),
        college: value('applicant-college'),
        major: value('applicant-major'),
        expectedGraduation: value('applicant-graduation'),
        property: value('property'),
        leaseStart: value('start-date'),
        leaseTerm: value('lease-term'),
        currentResidence: residence('current'),
        previousResidence: residence('previous'),
        evicted: value('evicted'),
        brokenLease: value('broken-lease'),
        bankruptcy: value('bankruptcy'),
        convicted: value('convicted'),
        explanation: value('explanation'),
        website: value('website'),
    }
}

export default function Application() {
    const [searchParams] = useSearchParams()
    const { data: properties } = useProperties()
    const [state, setState] = useState<SubmitState>('idle')
    const [errorMessage, setErrorMessage] = useState('')

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault()
        setState('sending')
        try {
            const res = await fetch('/api/applications/submit', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(toRequest(e.currentTarget)),
            })
            if (res.ok) {
                setState('sent')
                window.scrollTo({ top: 0 })
                return
            }
            setErrorMessage(res.status === 429
                ? 'Too many applications were sent from your connection. Please wait a few minutes and try again.'
                : 'Your application couldn’t be sent.')
        } catch {
            setErrorMessage('Your application couldn’t be sent. Check your internet connection.')
        }
        setState('error')
    }

    return (
        <SiteLayout>
            <PageHeader
                title="Rental application"
                intro="One application for any of our Norman or Stillwater properties."
                image="Norman_020_h7cj50.jpg"
                breadcrumbs={[{ to: '/', label: 'Home' }]}
            />

            <div className="container form-layout">
                <aside className="form-aside">
                    <h2>Before you start</h2>
                    <p>Have these ready:</p>
                    <ul>
                        <li>Driver's license number</li>
                        <li>Addresses and landlord contacts for the past two years</li>
                    </ul>
                    <p>
                        Questions? Call <a href={`tel:${CONTACT_PHONE}`}>{CONTACT_PHONE}</a> or
                        email <a href={`mailto:${CONTACT_EMAIL}`}>{CONTACT_EMAIL}</a>.
                    </p>
                </aside>

                {state === 'sent' ? (
                    <div className="panel" role="status">
                        <h2>Application sent</h2>
                        <p>
                            Thanks for applying. We've received your application and will contact you at the
                            email or phone number you gave us. If you have questions in the meantime, call{' '}
                            <a href={`tel:${CONTACT_PHONE}`}>{CONTACT_PHONE}</a>.
                        </p>
                        <Link className="button" to="/">Back to home</Link>
                    </div>
                ) : (
                <form className="application-form" onSubmit={handleSubmit}>
                    {/* Honeypot: hidden from people, filled in by spam bots. */}
                    <div className="hp-field" aria-hidden="true">
                        <label htmlFor="website">Website</label>
                        <input id="website" name="website" type="text" tabIndex={-1} autoComplete="off" />
                    </div>

                    <fieldset>
                        <legend><span>1</span>Personal information</legend>
                        <div className="form-grid">
                            <Field id="applicant-name" label="Full name" autoComplete="name" full required />
                            <Field id="applicant-email" label="Email" type="email" autoComplete="email" required />
                            <Field id="applicant-phone" label="Phone number" type="tel" autoComplete="tel" required />
                            <Field id="applicant-dob" label="Date of birth" type="date" autoComplete="bday" required />
                            <Field id="applicant-license" label="Driver's license number" maxLength={50} />
                        </div>
                    </fieldset>

                    <fieldset>
                        <legend><span>2</span>School</legend>
                        <div className="form-grid">
                            <Field id="applicant-college" label="College or university" full />
                            <Field id="applicant-major" label="Field of study / major" />
                            <Field id="applicant-graduation" label="Expected graduation date" type="date" />
                        </div>
                    </fieldset>

                    <fieldset>
                        <legend><span>3</span>Property and lease</legend>
                        <div className="form-grid">
                            <div className="field field--full">
                                <label htmlFor="property">Property</label>
                                <select id="property" name="property" defaultValue={searchParams.get('property') ?? ''} key={properties.length} required>
                                    <option value="" disabled>Select a property</option>
                                    {CAMPUSES.map(campus => (
                                        <optgroup key={campus.slug} label={campus.name}>
                                            {properties
                                                .filter(p => p.campus.toLowerCase() === campus.slug.toLowerCase())
                                                .map(p => <option key={p.id} value={p.name}>{p.name}</option>)}
                                        </optgroup>
                                    ))}
                                </select>
                            </div>
                            <Field id="start-date" label="Requested lease start date" type="date" />
                            <Field id="lease-term" label="Lease term" placeholder="e.g. 12 months" />
                        </div>
                    </fieldset>

                    <fieldset>
                        <legend><span>4</span>Residence history</legend>
                        <ResidenceFields prefix="current" heading="Current residence" />
                        <hr />
                        <ResidenceFields prefix="previous" heading="Previous residence (if applicable)" />
                    </fieldset>

                    <fieldset>
                        <legend><span>5</span>Background</legend>
                        <div className="question-list">
                            <YesNo name="evicted">Have you ever been evicted?</YesNo>
                            <YesNo name="broken-lease">Have you ever broken a lease?</YesNo>
                            <YesNo name="bankruptcy">Have you ever filed for bankruptcy?</YesNo>
                            <YesNo name="convicted">
                                Have you ever been convicted of a felony or misdemeanor (excluding minor traffic violations)?
                            </YesNo>
                        </div>
                        <div className="form-grid form-grid--spaced">
                            <div className="field field--full">
                                <label htmlFor="explanation">Explanation</label>
                                <span className="hint">If you answered yes to any question above, please explain.</span>
                                <textarea id="explanation" name="explanation" maxLength={2000} />
                            </div>
                        </div>
                    </fieldset>

                    <div className="form-submit">
                        <button type="submit" className="button" disabled={state === 'sending'}>
                            {state === 'sending' ? 'Sending…' : 'Submit application'}
                        </button>
                    </div>
                    {state === 'error' && (
                        <p className="form-status" role="alert">
                            {errorMessage} Please try again, or email it to us
                            at <a href={`mailto:${CONTACT_EMAIL}`}>{CONTACT_EMAIL}</a>.
                        </p>
                    )}
                </form>
                )}
            </div>
        </SiteLayout>
    )
}

interface FieldProps extends InputHTMLAttributes<HTMLInputElement> {
    id: string
    label: string
    full?: boolean
}

function Field({ id, label, full, type = 'text', ...inputProps }: FieldProps) {
    return (
        <div className={`field${full ? ' field--full' : ''}`}>
            <label htmlFor={id}>{label}</label>
            <input id={id} name={id} type={type} {...inputProps} />
        </div>
    )
}

function ResidenceFields({ prefix, heading }: { prefix: string; heading: string }) {
    return (
        <>
            <h3>{heading}</h3>
            <div className="form-grid">
                <Field id={`${prefix}-address`} label="Address" full />
                <Field id={`${prefix}-landlord`} label="Landlord or property manager" />
                <Field id={`${prefix}-landlord-contact`} label="Landlord phone or email" />
                <Field id={`${prefix}-from`} label="From" type="date" />
                <Field id={`${prefix}-to`} label="To" type="date" />
                <Field id={`${prefix}-reason`} label="Reason for leaving" full />
            </div>
        </>
    )
}

function YesNo({ name, children }: { name: string; children: ReactNode }) {
    return (
        <fieldset className="question">
            <legend>{children}</legend>
            <div className="question__options">
                <label><input type="radio" name={name} value="yes" /> Yes</label>
                <label><input type="radio" name={name} value="no" /> No</label>
            </div>
        </fieldset>
    )
}
