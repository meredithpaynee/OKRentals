
import { Cloudinary } from '@cloudinary/url-gen';
import { AdvancedImage } from '@cloudinary/react';
import { useNavigate } from 'react-router-dom'
import { useState } from 'react'
import './header.css';
import NavBar from './NavBar'
//import './logo.jpg'

function menu(menuOpen: boolean, setMenuOpen: (a: boolean) => void) {
    return (
    <button className="menu-toggle" aria-label="Menu" onClick={() => setMenuOpen(!menuOpen)} >
        <span></span>
        <span></span>
        <span></span>
    </button>
    )
}

function welcome(userName : string) {
    return (<span>Welcome, {userName}</span>)
}

function Header() {
    const loggedIn = false;
    const userName = "Meredith Payne"
    const [menuOpen, setMenuOpen] = useState(false);
    //const [menuVisible, setMenuVisible] = useState(true);
    const navigate = useNavigate();
    let right = null;

    const cld = new Cloudinary({ cloud: { cloudName: 'djt05lgzv' } });

    // Use this sample image or upload your own via the Media Library
    const img = cld
        .image('logo_cdwhmc.jpg')
        .format('auto')
        .quality('auto')

    if (!loggedIn)
    {
        right = menu(menuOpen, setMenuOpen);
    }
    else
    {
        right = welcome(userName);
    }


    return (
        <header className="header">
            <div className="header-content">
                <div className="logo" onClick={() => navigate('/')}>
                    <AdvancedImage className = 'logo-icon'cldImg={img} />
                    <span className="logo-text">Oklahoma Rentals</span>
                </div>
                {right}
            </div>
            {menuOpen && <NavBar />}
        </header>
    );
}

export default Header;
