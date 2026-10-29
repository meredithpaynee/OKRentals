import * as React from 'react';
import { Cloudinary } from '@cloudinary/url-gen';
import { AdvancedImage } from '@cloudinary/react';
import Header from './Header';
import './about.css';

export default function AboutPage() {
    const cld = new Cloudinary({ cloud: { cloudName: 'djt05lgzv' } });

    const img1 = cld.image('stillwater-downtown.jpg').format('auto').quality('auto');
    const img2 = cld.image('oklahoma-landscape_jblnki.jpg').format('auto').quality('auto');
    const img3 = cld.image('stillwater-map.jpg').format('auto').quality('auto');
    const img4 = cld.image('norman-map.jpg').format('auto').quality('auto');

    const heroStyle = img1
        ? {
            backgroundImage: `url(${img1.toURL()})`,
            backgroundSize: 'cover',
            backgroundPosition: 'center',
            backgroundRepeat: 'no-repeat',
        } as React.CSSProperties
        : undefined;

    return (
        <div className="about-container">
            <Header />

            {/* Hero */}
            <div className="hero-section" style={heroStyle}>
                <div className="hero-overlay" />
                <div className="hero-content">
                    <p className="hero-eyebrow">Welcome to</p>
                    <h1 className="hero-title">Oklahoma College Rentals</h1>
                </div>
            </div>

            {/* Section 1 — Landscape image + intro copy */}
            <section className="about-section">
                <div className="about-image-wrap">
                    <AdvancedImage className="about-image" cldImg={img2} />
                </div>
                <div className="about-text">
                    <span className="section-label">Who We Are</span>
                    <p>Oklahoma College Rentals is a family-run rental property company specializing in student housing across Oklahoma.</p>
                </div>
            </section>

            {/* Section 2 — copy + two maps */}
            <section className="about-section reverse">
                <div className="about-text">
                    <span className="section-label">Our Promise</span>
                    <p>We pride ourselves on high-quality properties, strong customer service, and genuine personal connection with every tenant.</p>
                </div>
                <div className="about-image-wrap">
                    <AdvancedImage className="about-image" cldImg={img3} />
                </div>
                <div className="about-image-wrap">
                    <AdvancedImage className="about-image" cldImg={img4} />
                </div>
            </section>

            {/* Section 3 — CTA strip */}
            <section className="about-cta">
                <p>Our goal is to help you find the right home for your needs!</p>
            </section>
        </div>
    );
}