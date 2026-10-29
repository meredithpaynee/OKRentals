import * as React from "react"
import { Cloudinary } from '@cloudinary/url-gen';
import { useNavigate } from 'react-router-dom' 
import './FrontPage.css';
import Header from './Header';

export default function FrontPage() {
    const navigate = useNavigate();

    const cld = new Cloudinary({ cloud: { cloudName: 'djt05lgzv' } });

    const img = cld
        .image('frontpage_bb4k0h.png')
        .format('auto')
        .quality('auto')

    const heroStyle = img
        ? {
            backgroundImage: `url(${img.toURL()})`,
            backgroundSize: 'cover',
            backgroundPosition: 'center',
            backgroundRepeat: 'no-repeat'
        } as React.CSSProperties
        : undefined;

    return (
        <div className="frontpage-container">
            <Header/>
            
            <div className="hero-section" style={heroStyle }>
                <div className="hero-overlay"></div>
                <div className="hero-content">
                    <h1 className="hero-title">Oklahoma College Rentals</h1>
                </div>
            </div>

            <div className="content-section">
                <p className="subtitle">
                    Properties on both the University of Oklahoma and Oklahoma State University campuses. 
                    Affordable student living.
                </p>
                
                <div className="cta-buttons">
                    <button className="cta-btn norman-btn" onClick={() => navigate('/properties/Norman')}>
                    Norman Properties
                    </button>
                    <button className="cta-btn stillwater-btn" onClick={() => navigate('/properties/Stillwater')}>
                    Stillwater Properties
                    </button>
                </div>
            </div>
        </div>
    );
}
