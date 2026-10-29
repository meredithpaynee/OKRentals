import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import SiteLayout from './site/SiteLayout'
import { CONTACT_EMAIL, CONTACT_PHONE, findCampus } from './site/siteInfo'
import { availabilityLabel, fetchImages, formatRent, type CloudinaryImage, type PropertyResponse } from './site/api'
import { sizedUrl } from './site/images'

export default function Property() {
    const { name } = useParams<{ name: string }>()
    const [property, setProperty] = useState<PropertyResponse | null>(null)
    const [images, setImages] = useState<CloudinaryImage[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        if (!name) return

        fetch(`/api/properties/byName/${encodeURIComponent(name)}`)
            .then(res => {
                if (!res.ok) throw new Error('Property not found')
                return res.json()
            })
            .then((data: PropertyResponse) => {
                setProperty(data)
                // Photos are optional; a failure here shouldn't hide the listing.
                return fetchImages(data.name).then(setImages).catch(() => setImages([]))
            })
            .catch(err => setError(err.message))
            .finally(() => setLoading(false))
    }, [name])

    if (loading) {
        return (
            <SiteLayout>
                <div className="container notice" aria-busy="true">Loading property…</div>
            </SiteLayout>
        )
    }

    if (error || !property) {
        return (
            <SiteLayout>
                <div className="container notice">
                    <h2>We couldn't find that property</h2>
                    <p>It may have been removed. <Link to="/">Browse all properties</Link>.</p>
                </div>
            </SiteLayout>
        )
    }

    const campus = findCampus(property.campus)

    return (
        <SiteLayout>
            <div className="container property-title">
                <nav className="breadcrumbs" aria-label="Breadcrumb">
                    <span><Link to="/">Home</Link><span aria-hidden="true">/</span></span>
                    {campus && (
                        <span><Link to={`/properties/${campus.slug}`}>{campus.name}</Link><span aria-hidden="true">/</span></span>
                    )}
                    <span aria-current="page">{property.name}</span>
                </nav>
                <h1>{property.name}</h1>
                <p>{property.address}</p>
            </div>

            <div className="container">
                <Gallery images={images} propertyName={property.name} />
            </div>

            <div className="container property-body">
                <div>
                    <h2>Details</h2>
                    <table className="spec-table">
                        <tbody>
                            <tr><th scope="row">Rent</th><td>{formatRent(property.price)} per month</td></tr>
                            <tr><th scope="row">Bedrooms</th><td>{property.bedrooms}</td></tr>
                            <tr><th scope="row">Bathrooms</th><td>{property.bathrooms}</td></tr>
                            <tr><th scope="row">Availability</th><td>{availabilityLabel(property)}</td></tr>
                            {campus && <tr><th scope="row">Campus</th><td>{campus.name} ({campus.school})</td></tr>}
                            <tr><th scope="row">Address</th><td>{property.address}</td></tr>
                        </tbody>
                    </table>

                    {property.description && (
                        <>
                            <h2>About this property</h2>
                            <p className="property-description">{property.description}</p>
                        </>
                    )}
                </div>

                <aside className="aside-box">
                    <div className="aside-box__price">{formatRent(property.price)}<span> / month</span></div>
                    <div className={`status${property.isLeased ? '' : ' status--available'}`}>
                        {availabilityLabel(property)}
                    </div>
                    <Link className="button button--block" to={`/apply?property=${encodeURIComponent(property.name)}`}>
                        Apply for this property
                    </Link>
                    <div className="aside-box__contact">
                        Questions or want a showing?
                        <a href={`tel:${CONTACT_PHONE}`}>{CONTACT_PHONE}</a>
                        <a href={`mailto:${CONTACT_EMAIL}`}>{CONTACT_EMAIL}</a>
                    </div>
                </aside>
            </div>
        </SiteLayout>
    )
}

function Gallery({ images, propertyName }: { images: CloudinaryImage[]; propertyName: string }) {
    const [current, setCurrent] = useState(0)

    if (images.length === 0) {
        return <div className="gallery__empty">Photos coming soon</div>
    }

    const prev = () => setCurrent(i => (i === 0 ? images.length - 1 : i - 1))
    const next = () => setCurrent(i => (i === images.length - 1 ? 0 : i + 1))

    return (
        <div className="gallery">
            <div className="gallery__main">
                <img src={sizedUrl(images[current].secureUrl, 1600)} alt={`${propertyName}, photo ${current + 1}`} />
                {images.length > 1 && (
                    <>
                        <button className="gallery__nav gallery__nav--prev" onClick={prev} aria-label="Previous photo">‹</button>
                        <button className="gallery__nav gallery__nav--next" onClick={next} aria-label="Next photo">›</button>
                        <span className="gallery__count">{current + 1} / {images.length}</span>
                    </>
                )}
            </div>
            {images.length > 1 && (
                <div className="gallery__thumbs">
                    {images.map((img, i) => (
                        <button
                            key={img.publicId}
                            className="gallery__thumb"
                            aria-current={i === current}
                            aria-label={`Show photo ${i + 1}`}
                            onClick={() => setCurrent(i)}
                        >
                            <img src={sizedUrl(img.secureUrl, 240)} alt="" loading="lazy" />
                        </button>
                    ))}
                </div>
            )}
        </div>
    )
}
