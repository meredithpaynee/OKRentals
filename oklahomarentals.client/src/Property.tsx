import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom'
import { useNavigate } from 'react-router-dom'
import Header from "./Header";
import './properties.css'

interface PropertyResponse {
    id: number
    name: string
    campus: string
    bedrooms: number
    bathrooms: number
    price: number
    address: string
    isLeased: boolean
    description: string
    nextAvailable: string
}
interface CloudinaryImage {
    publicId: string
    secureUrl: string
}

export default function Property() {
    const navigate = useNavigate();
    const { name } = useParams<{ name: string }>();
    const [property, setProperty] = useState<PropertyResponse | null>(null);
    const [images, setImages] = useState<CloudinaryImage[]>([]);
    const [currentIndex, setCurrentIndex] = useState(0);

    const prev = () => setCurrentIndex(i => (i === 0 ? images.length - 1 : i - 1));
    const next = () => setCurrentIndex(i => (i === images.length - 1 ? 0 : i + 1));

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!name) return;

        fetch(`/api/properties/byName/${encodeURIComponent(name ?? '')}`)
            .then(res => {
                if (!res.ok) throw new Error('Property not found')
                return res.json()
            })
            .then((data: PropertyResponse) => {
                setProperty(data);
                return fetch(`/api/properties/${encodeURIComponent(data.name)}/images`);
            })
            .then(res => {
                if (!res.ok) throw new Error('Failed to fetch images')
                return res.json()
            })
            .then((imgs: CloudinaryImage[]) => setImages(imgs))
            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, [name]);

    if (loading) return <div>Loading...</div>
    if (error) return <div>Error: {error}</div>
    if (!property) return <div>Property not found.</div>

    // Format NextAvailable to "Month day, Year" (e.g. "April 27, 2026").
    const formattedNextAvailable = (() => {
        if (!property.nextAvailable) return 'TBD';
        const d = new Date(property.nextAvailable);
        if (isNaN(d.getTime()) || d.getFullYear() < 1900) return 'TBD';
        return d.toLocaleDateString(undefined, { year: 'numeric', month: 'long', day: 'numeric' });
    })();

  return (
    <div className="property-container">
      <Header/>
          <div key={property.id}>
              <div className="hero-section">
                  <div className="hero-overlay"></div>
                  <div className="hero-content">
                      <h1 className='hero-title'>{property.name}</h1>
                  </div>
              </div>
              <div className='content'>
                  <h3>{property.bedrooms} bed / {property.bathrooms} bath</h3>
                  <h3> - ${property.price}/mo</h3>
                  <h3> - {property.isLeased ? 'Leased' : 'Available'}</h3>
                  <h3> - Available {formattedNextAvailable}</h3>
              </div>
          </div>
          <div className="property-images">
              {images.length > 0 && (
                  <div className = 'carousel-container'>
                      <div className='carousel'>
                        {[-1,0,1].map(offset => {
                            const idx = (currentIndex + offset + images.length) % images.length;
                            return (
                                <img
                                key={images[idx].publicId}
                                src={images[idx].secureUrl}
                                alt={`${property.name} ${idx + 1}`}
                                style={{ width: '32%', height: '400px', objectFit: 'cover', borderRadius: '8px' }}
                                />
                            )
                        }) }
                        </div>
                      {images.length > 1 && (
                          <>
                              <button
                                  className={property.campus == "Norman" ? 'left-button-nor' : 'left-button-still'}
                                  onClick={prev}
                              >‹</button>
                              <button
                                  className={property.campus == "Norman" ? 'right-button-nor' : 'right-button-still'}
                                  onClick={next}
                              >›</button>
                              <div style={{ marginTop: '0.5rem', color: '#666' }}>
                                  {currentIndex + 1} / {images.length}
                              </div>
                          </>
                      )}
                  </div>
              )}
              {images.length === 0 && <p>No images available.</p>}
          </div>
          <div className="Description">
              <h2>Description</h2>
              <span>{property.description}</span>
          </div>
      <button onClick={() => navigate(`/apply`)}>Apply Now</button>
    </div>
  );
}