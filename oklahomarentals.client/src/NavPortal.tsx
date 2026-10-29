//import { useNavigate } from 'react-router-dom'
import * as React from 'react';
import './navportal.css'

export default function NavPortal() {
    return (
        <div className="navbar">
            <a className='navbar-btn' href='overview'>Home</a>
            <a className='navbar-btn' href='#payments'>Payment History</a>
            <a className='navbar-btn' href='#maintenance'>Maintenance</a>
            <a className='navbar-btn' href='#documents'>Documents</a>
        </div>);
}