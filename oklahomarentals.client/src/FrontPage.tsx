import type { CSSProperties } from 'react'
import { Link } from 'react-router-dom'
import SiteLayout, { ContactBand } from './site/SiteLayout'
import { CAMPUSES } from './site/siteInfo'
import { cloudinaryUrl } from './site/images'

const campusColors: Record<string, string> = { Norman: 'var(--norman)', Stillwater: 'var(--stillwater)' }

export default function FrontPage() {
    return (
        <SiteLayout>
            <section className="hero" style={{ backgroundImage: `url(${cloudinaryUrl('frontpage_bb4k0h.png', 2400)})` }}>
                <div className="container">
                    <h1>Student rentals near OU and OSU</h1>
                    <p>
                        Houses and apartments in Norman and Stillwater from a family-run company
                        that specializes in student housing.
                    </p>
                    <a className="button button--light" href="#campuses">See available properties</a>
                </div>
            </section>

            <section className="section" id="campuses">
                <div className="container">
                    <div className="section-heading">
                        <h2>Choose a campus</h2>
                        <p>Every listing shows rent, bedrooms, and the date it's next available.</p>
                    </div>
                    <div className="campus-grid">
                        {CAMPUSES.map(campus => (
                            <Link
                                key={campus.slug}
                                to={`/properties/${campus.slug}`}
                                className="campus-card"
                                style={{ '--campus-color': campusColors[campus.slug] } as CSSProperties}
                            >
                                <div
                                    className="campus-card__image"
                                    style={{ backgroundImage: `url(${cloudinaryUrl(campus.image, 1200)})` }}
                                />
                                <div className="campus-card__body">
                                    <h3>{campus.name}</h3>
                                    <p>{campus.school}</p>
                                    <span className="campus-card__link">View {campus.name} properties &rarr;</span>
                                </div>
                            </Link>
                        ))}
                    </div>
                </div>
            </section>

            <section className="section section--surface">
                <div className="container split">
                    <div>
                        <span className="eyebrow">About us</span>
                        <h2>A small, family-run landlord</h2>
                        <p>
                            Oklahoma College Rentals is a family-run rental company specializing in student
                            housing. We focus on well-kept properties, good customer service, and a genuine
                            personal connection with every tenant.
                        </p>
                        <Link className="text-link" to="/about">More about us &rarr;</Link>
                    </div>
                    <ul className="fact-list">
                        <li>
                            <strong>Apply online</strong>
                            <span>One application for any of our properties.</span>
                        </li>
                        <li>
                            <strong>Resident portal</strong>
                            <span>See your payment history and documents, and submit maintenance requests.</span>
                        </li>
                        <li>
                            <strong>Two college towns</strong>
                            <span>Properties near the University of Oklahoma and Oklahoma State University.</span>
                        </li>
                    </ul>
                </div>
            </section>

            <ContactBand />
        </SiteLayout>
    )
}
