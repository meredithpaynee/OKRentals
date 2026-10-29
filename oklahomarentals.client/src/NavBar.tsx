import * as React from 'react';
import { useNavigate } from 'react-router-dom'
import { useState } from 'react'
import './navbar.css'
import { useAuth } from 'react-oidc-context'

export default function NavBar() {
    const navigate = useNavigate()
    const auth = useAuth()
    const [showProperties, setShowProperties] = useState(false)

    return (
        <div className='navbar'>
            <button className='navbar-btn' onClick={() => setShowProperties(!showProperties)}>Properties</button>
            {showProperties && (
                <div className='sub-links'>
                    <button className='navbar-btn sub-btn' onClick={() => navigate('/properties/Norman')}>Norman</button>
                    <button className='navbar-btn sub-btn' onClick={() => navigate('/properties/Stillwater')}>Stillwater</button>
                </div>
            )}
            <button className='navbar-btn' onClick={() => navigate('/about')}>About</button>
            <button className='navbar-btn' onClick={() => navigate('/contact')}>Contact</button>
            <button className='navbar-btn' onClick={() => navigate('/apply')}>Apply to Rent</button>
            
            <button className='navbar-btn' onClick={() => auth.isAuthenticated ? navigate('/portal') : auth.signinRedirect()}>
                Resident Portal
            </button>
        </div>
    );
}