import { BrowserRouter, Routes, Route } from 'react-router-dom'
import FrontPage from './FrontPage'
import About from './AboutPage'
import ContactPage from './ContactPage'
import Properties from './Properties'
import Application from './Application'
import Login from './Login'
import PortalHome from './PortalHome'
import AdminPortal from './AdminPortal'
import Property from './Property'
import Payment from './Payment'
import './App.css'
import ProtectedRoute from './ProtectedRoute'

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<FrontPage />/*<PortalHome access = "User" address= "1405 Garfield Avenue, Norman, OK, 12345 " />*/} />
                <Route path="/about" element={<About />} />
                <Route path="/contact" element={<ContactPage email="okcollegerentals@gmail.com" phone="214-202-0008" />} />
                <Route path="/properties/:location" element={<Properties />} />
                <Route path="/apply" element={<Application />} />
                <Route path="/login" element={<Login />} />
                <Route path="/property/:name" element={<Property />} />
                <Route path="/apply" element={<Application />} />


                <Route path="/portal/:access" element={<PortalHome access={""} address="1405 Garland Ave" />} />
                <Route path="/portal/admin" element={
                    <ProtectedRoute>
                        <AdminPortal />
                    </ProtectedRoute>
                } />
                <Route path="/:username/make-payment" element={<Payment />} />

                <Route path="/properties/:location" element={<Properties />} />
                <Route path="/portal" element={
                    <ProtectedRoute>
                        <PortalHome access="Tenant" address="" />
                    </ProtectedRoute>
                } />

                {/* Protected routes - add routes requiring authentication here */}
                {/* Example:
                <Route path="/dashboard" element={
                    <ProtectedRoute>
                        <Dashboard />
                    </ProtectedRoute>
                } /> */}
            </Routes>
        </BrowserRouter>
    )
}