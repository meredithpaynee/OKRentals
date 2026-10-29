import { Cloudinary } from '@cloudinary/url-gen';
import { AdvancedImage } from '@cloudinary/react';
import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { useNavigate } from 'react-router-dom'
import Header from './Header'
import './properties.css'

interface PropertyResponse {
    id: number
    name: string
    campus: string
    bedrooms: number
    bathrooms: number
    price: number
    address: string
    description: string
    isLeased: boolean
    nextAvailable: string
}

interface CloudinaryImage {
    publicId: string
    secureUrl: string
}

export default function Properties() {
    const navigate = useNavigate();
    const { location: routeLocation } = useParams<{ location?: string }>();
    const [properties, setProperties] = useState<PropertyResponse[]>([]);

    const [images, setImages] = useState<CloudinaryImage[]>([]);
    const [currentIndex, setCurrentIndex] = useState(0);

    const prev = () => setCurrentIndex(i => (i === 0 ? images.length - 1 : i - 1));
    const next = () => setCurrentIndex(i => (i === images.length - 1 ? 0 : i + 1));


    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);



    useEffect(() => {
        fetch('/api/properties')
            .then(res => {
                if (!res.ok) throw new Error('Failed to fetch properties')
                return res.json()
            })
            .then((data: PropertyResponse[]) => {
                const filtered = routeLocation
                    ? data.filter(p => p.campus.toLowerCase() === routeLocation.toLowerCase())
                    : data;
                setProperties(filtered);
            })
            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, [routeLocation]);

    if (loading) return <div>Loading...</div>
    if (error) return <div>Error: {error}</div>

    const cld = new Cloudinary({ cloud: { cloudName: 'djt05lgzv' } });

    // Use this sample image or upload your own via the Media Library
    let img;

    if (routeLocation.toLowerCase() == "norman") {
        img = cld
            .image('norman-enter_l8zewm.jpg')
            .format('auto')
            .quality('auto')
    }
    else {
        img = cld
            .image('stillwater-enter_hbatzr.jpg')
            .format('auto')
            .quality('auto')
    }

    const heroStyle = img
        ? {
            backgroundImage: `url(${img.toURL()})`,
            backgroundSize: 'cover',
            backgroundPosition: 'center',
            backgroundRepeat: 'no-repeat'
        } as React.CSSProperties
        : undefined;

    return (
        <div className='properties-container'>
            <Header />
            <div className="hero-section" style={heroStyle}>
                <div className="hero-overlay"></div>
                <div className="hero-content">
                    <h1 className = 'hero-title'>{routeLocation ? `${routeLocation} Properties` : 'Properties'}</h1>
                </div>
            </div>

            {properties.map((property) => (
                <div className='properties' key={property.id}>
                    <div className='property-info'>
                        <div className='property-content'>
                            <h2>{property.name}</h2>
                            <p>{property.bedrooms} bed / {property.bathrooms} bath</p>
                        </div>
                        <button onClick={() => navigate(`/property/${property.name}`)}>More information</button>
                    </div>
                    <hr className = 'divider'></hr>
                </div>
            ))}

            {properties.length === 0 && <p>No properties found for this campus.</p>}
        </div>
    );
}