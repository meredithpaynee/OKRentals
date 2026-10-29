import type { ReactNode } from 'react'
import { Link } from 'react-router-dom'
import Header from '../Header'
import { CAMPUSES, CONTACT_EMAIL, CONTACT_PHONE, SITE_NAME } from './siteInfo'
import { cloudinaryUrl } from './images'
import './site.css'

// Shell for every public page: header, page content, footer.
export default function SiteLayout({ children }: { children: ReactNode }) {
    return (
        <div className="site">
            <Header />
            <main>{children}</main>
            <Footer />
        </div>
    )
}

interface PageHeaderProps {
    title: string
    intro?: string
    image?: string
    breadcrumbs?: { to: string; label: string }[]
}

// Title band at the top of inner pages, optionally over a photo.
export function PageHeader({ title, intro, image, breadcrumbs }: PageHeaderProps) {
    return (
        <section
            className={`page-header${image ? ' page-header--image' : ''}`}
            style={image ? { backgroundImage: `url(${cloudinaryUrl(image, 2000)})` } : undefined}
        >
            <div className="container">
                {breadcrumbs && (
                    <nav className="breadcrumbs" aria-label="Breadcrumb">
                        {breadcrumbs.map(crumb => (
                            <span key={crumb.to}>
                                <Link to={crumb.to}>{crumb.label}</Link>
                                <span aria-hidden="true">/</span>
                            </span>
                        ))}
                        <span aria-current="page">{title}</span>
                    </nav>
                )}
                <h1>{title}</h1>
                {intro && <p className="page-header__intro">{intro}</p>}
            </div>
        </section>
    )
}

// Dark call-to-action strip used at the bottom of several pages.
export function ContactBand() {
    return (
        <section className="contact-band">
            <div className="container">
                <div>
                    <h2>Questions about a property?</h2>
                    <p>
                        Call <a href={`tel:${CONTACT_PHONE}`}>{CONTACT_PHONE}</a> or email{' '}
                        <a href={`mailto:${CONTACT_EMAIL}`}>{CONTACT_EMAIL}</a>.
                    </p>
                </div>
                <Link className="button button--light" to="/apply">Apply to rent</Link>
            </div>
        </section>
    )
}

function Footer() {
    return (
        <footer className="site-footer">
            <div className="container site-footer__grid">
                <div>
                    <p className="site-footer__name">{SITE_NAME}</p>
                    <p>Family-run student housing in Norman and Stillwater, Oklahoma.</p>
                </div>
                <div>
                    <h2>Properties</h2>
                    <ul>
                        {CAMPUSES.map(c => (
                            <li key={c.slug}><Link to={`/properties/${c.slug}`}>{c.name}</Link></li>
                        ))}
                    </ul>
                </div>
                <div>
                    <h2>Company</h2>
                    <ul>
                        <li><Link to="/about">About</Link></li>
                        <li><Link to="/contact">Contact</Link></li>
                        <li><Link to="/apply">Apply to rent</Link></li>
                    </ul>
                </div>
                <div>
                    <h2>Contact</h2>
                    <ul>
                        <li><a href={`tel:${CONTACT_PHONE}`}>{CONTACT_PHONE}</a></li>
                        <li><a href={`mailto:${CONTACT_EMAIL}`}>{CONTACT_EMAIL}</a></li>
                    </ul>
                </div>
            </div>
            <div className="container site-footer__legal">
                <p>&copy; {new Date().getFullYear()} {SITE_NAME}</p>
            </div>
        </footer>
    )
}
