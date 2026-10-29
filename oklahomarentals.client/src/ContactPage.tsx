import { Cloudinary } from '@cloudinary/url-gen';
import { AdvancedImage } from '@cloudinary/react';
import * as React from 'react';
import './contactpage.css'
import Header from './Header'
import { useNavigate } from 'react-router-dom'

interface props {
    email: string;
    phone: string;
}

export default function ContactPage(props: props) {
    const navigate = useNavigate();

    const cld = new Cloudinary({ cloud: { cloudName: 'djt05lgzv' } });

    const img = cld
        .image('norman-downtown_ys47pm.jpg')
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
        <div className="contact-page">
            <Header />
            <div className="hero-section" style={heroStyle}>
                <div className="hero-overlay"></div>
                <div className="hero-content">
                    <h1 className="hero-title">Contact Us</h1>
                </div>
            </div>
            

            <h2> Have an inquiry about a property or renting? Contact us at:</h2>
            <div className='contactInfo'>
                <div className='contact-card'>
                    <h4>Email</h4>
                    <p> {props.email}</p>
                </div>
                <div className='contact-card'>
                    <h4>Call</h4>
                    <p> {props.phone}</p>
                </div>
                <div className='contact-card'>
                    <h4>Apply</h4>
                    <button className="btn" onClick={() => navigate('/apply')}>Apply to Rent</button>
                </div>
            </div>
        </div>
    )
}