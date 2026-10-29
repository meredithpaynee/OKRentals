import { useState } from 'react'
import { Link, NavLink, useNavigate } from 'react-router-dom'
import { CAMPUSES, CONTACT_EMAIL, CONTACT_PHONE, SITE_NAME } from './site/siteInfo'
import './header.css'

export default function Header() {
    const navigate = useNavigate()
    const [menuOpen, setMenuOpen] = useState(false)

    // /portal is a ProtectedRoute, which sends signed-out visitors to Keycloak
    // and shows an error if Keycloak can't be reached.
    const openPortal = () => {
        setMenuOpen(false)
        navigate('/portal')
    }

    const links = [
        ...CAMPUSES.map(c => ({ to: `/properties/${c.slug}`, label: c.name })),
        { to: '/about', label: 'About' },
        { to: '/contact', label: 'Contact' },
        { to: '/apply', label: 'Apply' },
    ]

    return (
        <header className="site-header">
            <div className="site-header__utility">
                <div className="site-header__inner">
                    <span>Student rentals in Norman and Stillwater</span>
                    <span className="site-header__contact">
                        <a href={`tel:${CONTACT_PHONE}`}>{CONTACT_PHONE}</a>
                        <a href={`mailto:${CONTACT_EMAIL}`}>{CONTACT_EMAIL}</a>
                    </span>
                </div>
            </div>

            <div className="site-header__inner site-header__main">
                <Link to="/" className="site-header__brand" onClick={() => setMenuOpen(false)}>
                    <img src="/logo.jpg" alt="" width="36" height="36" />
                    <span>{SITE_NAME}</span>
                </Link>

                <button
                    className="site-header__toggle"
                    aria-expanded={menuOpen}
                    aria-controls="site-nav"
                    onClick={() => setMenuOpen(!menuOpen)}
                >
                    {menuOpen ? 'Close' : 'Menu'}
                </button>

                <nav id="site-nav" className={`site-header__nav${menuOpen ? ' is-open' : ''}`} aria-label="Main">
                    {links.map(link => (
                        <NavLink key={link.to} to={link.to} onClick={() => setMenuOpen(false)}>
                            {link.label}
                        </NavLink>
                    ))}
                    <button className="site-header__portal" onClick={openPortal}>Resident portal</button>
                </nav>
            </div>
        </header>
    )
}
