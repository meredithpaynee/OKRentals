import { useEffect, useState } from 'react'
import { Link, NavLink, useParams } from 'react-router-dom'
import SiteLayout, { PageHeader } from './site/SiteLayout'
import { CAMPUSES, findCampus } from './site/siteInfo'
import { availabilityLabel, fetchImages, formatRent, useProperties, type PropertyResponse } from './site/api'
import { sizedUrl } from './site/images'

export default function Properties() {
    const { location } = useParams<{ location?: string }>()
    const campus = findCampus(location)
    const { data, loading, error } = useProperties()

    const properties = campus ? data.filter(p => p.campus.toLowerCase() === campus.slug.toLowerCase()) : data
    const title = campus ? `${campus.name} properties` : 'Properties'

    return (
        <SiteLayout>
            <PageHeader
                title={title}
                intro={campus ? `Rentals near the ${campus.school}.` : undefined}
                image={campus?.image}
                breadcrumbs={[{ to: '/', label: 'Home' }]}
            />

            <section className="section">
                <div className="container">
                    <div className="listing-toolbar">
                        <nav className="campus-tabs" aria-label="Campus">
                            {CAMPUSES.map(c => (
                                <NavLink key={c.slug} to={`/properties/${c.slug}`}>{c.name}</NavLink>
                            ))}
                        </nav>
                        {!loading && !error && (
                            <span className="listing-count">
                                {properties.length} {properties.length === 1 ? 'property' : 'properties'}
                            </span>
                        )}
                    </div>

                    {loading && (
                        <div className="listing-grid" aria-busy="true">
                            {[0, 1, 2].map(i => <div key={i} className="skeleton" />)}
                        </div>
                    )}

                    {error && (
                        <div className="notice">
                            <h2>We couldn't load properties right now</h2>
                            <p>Please refresh the page, or <Link to="/contact">contact us</Link> for current availability.</p>
                        </div>
                    )}

                    {!loading && !error && properties.length === 0 && (
                        <div className="notice">
                            <h2>No properties listed here yet</h2>
                            <p>Check back soon, or <Link to="/contact">contact us</Link> about upcoming availability.</p>
                        </div>
                    )}

                    {!loading && !error && properties.length > 0 && (
                        <div className="listing-grid">
                            {properties.map(property => <ListingCard key={property.id} property={property} />)}
                        </div>
                    )}
                </div>
            </section>
        </SiteLayout>
    )
}

function ListingCard({ property }: { property: PropertyResponse }) {
    const [cover, setCover] = useState<string | null>(null)

    useEffect(() => {
        fetchImages(property.name)
            .then(imgs => setCover(imgs[0]?.secureUrl ?? null))
            .catch(() => setCover(null))
    }, [property.name])

    return (
        <Link className="listing-card" to={`/property/${encodeURIComponent(property.name)}`}>
            {cover
                ? <img className="listing-card__image" src={sizedUrl(cover, 800)} alt="" loading="lazy" />
                : <div className="listing-card__image" />}
            <div className="listing-card__body">
                <span className={`status${property.isLeased ? '' : ' status--available'}`}>
                    {availabilityLabel(property)}
                </span>
                <h3>{property.name}</h3>
                <span className="listing-card__address">{property.address}</span>
                <div className="listing-card__meta">
                    <span className="listing-card__price">{formatRent(property.price)}<span> /mo</span></span>
                    <span className="listing-card__specs">{property.bedrooms} bd · {property.bathrooms} ba</span>
                </div>
            </div>
        </Link>
    )
}
