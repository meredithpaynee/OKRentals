import * as React from 'react'
import './portal.css'
import Header from "./Header"
import { useState, useEffect } from 'react'
import NavPortal from "./NavPortal"
import { CreateProperty, RemoveProperty, Tenants, PreviousPayments, MaintenanceRequests, CreateTenant, RemoveTenant, SetPropertyToLeased, RecordPayment } from './AdminPortalFunctionality'


interface PropertyResponse {
    id: number
    name: string
    campus: string
    bedrooms: number
    bathrooms: number
    price: number
    address: string
    isLeased: boolean
}
export default function AdminPortal() {
    const [properties, setProperties] = useState<PropertyResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [showCreateProperty, setShowCreateProperty] = useState(false);
    const [showRemoveProperty, setShowRemoveProperty] = useState(false);
    const [showCreateTenant, setShowCreateTenant] = useState(false);
    const [showRemoveTenant, setShowRemoveTenant] = useState(false);
    const [showRecordPayment, setShowRecordPayment] = useState(false);
    const [showToLeased, setShowToLeased] = useState(false);

    // collapsed by default: only title visible
    const [expandedCards, setExpandedCards] = useState<Record<string, boolean>>({
        editing: false,
        payments: false,
        maintenance: false,
        documents: false,
        tenants: false,
    });

    const toggleCard = (key: string) => {
        setExpandedCards(prev => ({ ...prev, [key]: !prev[key] }));
    }

    useEffect(() => {
        fetch('/api/properties')
            .then(res => {
                if (!res.ok) throw new Error('Failed to fetch properties')
                return res.json()
            })
            .then((data: PropertyResponse[]) => {
                setProperties(data);
            })
            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, []);

    const handlePropertyDeleted = (id: number) => {
        setProperties(prev => prev.filter(p => p.id !== id));
    }

    return (
        <div className='portal'>
            <Header />
            <NavPortal />
            <div className='top'>
                <h1>Admin Portal</h1>
            </div>
            <div className='lower'>
                <div className='left-column'>
                    <div className='content-card'>
                        <div className="card-header">
                        </div>
                        <div className="editing-buttons">
                            <button onClick={() => setShowCreateProperty(true)} className="editing-button">Create Property</button>
                            {showCreateProperty && <>
                                <div className="modal-backdrop" />
                                <div className="modal-popup request-card">
                                    <CreateProperty />
                                </div>
                                <hr className="divider" />
                            </>}
                            <button onClick={() => setShowRemoveProperty(true)} className="editing-button">Remove Property</button>
                            {showRemoveProperty && <>
                                <div className="modal-backdrop" />
                                <div className="modal-popup request-card">
                                    <RemoveProperty properties={properties} onDeleted={handlePropertyDeleted} />
                                </div>
                                <hr className="divider" />
                            </>}
                            <button onClick={() => setShowCreateTenant(true)} className="editing-button">Create Tenant</button>
                            {showCreateTenant && <>
                                <div className="modal-backdrop" />
                                <div className="modal-popup request-card">
                                    <CreateTenant />
                                </div>
                                <hr className="divider" />
                            </>}
                            <button onClick={() => setShowRemoveTenant(true)} className="editing-button" >Remove Tenant</button>
                            {showRemoveTenant && <>
                                <div className="modal-backdrop" />
                                <div className="modal-popup request-card">
                                    <RemoveTenant />
                                </div>
                                <hr className="divider" />
                            </>}
                            <button onClick={() => setShowToLeased(true)} className="editing-button">Set Property To Leased</button>
                            {showToLeased && <>
                                <div className="modal-backdrop" />
                                <div className="modal-popup request-card">
                                    <SetPropertyToLeased />
                                </div>
                                <hr className="divider" />
                            </>}
                            <button onClick={() => setShowRecordPayment(true)} className="editing-button" >Record Payment</button>
                            {showRecordPayment && <>
                                <div className="modal-backdrop" />
                                <div className="modal-popup request-card">
                                    <RecordPayment />
                                </div>
                                <hr className="divider" />
                            </>}
                        </div>
                    </div>
                    <div className='content-card'>
                        <div className="card-header">
                            <h2>Payment History</h2>
                            <button className="collapse-btn" onClick={() => toggleCard('payments')}>{expandedCards.payments ? '▲' : '▼'}</button>
                        </div>
                        <div className={`card-body ${expandedCards.payments ? 'expanded' : 'collapsed'}`}>
                            {properties.map((property) => (
                                <div className='properties' key={property.id}>
                                    <div className='property-info'>
                                        <h4>{property.name}</h4>
                                        <PreviousPayments property={property.id} />
                                    </div>
                                    <hr className='divider'></hr>
                                </div>
                            ))}
                        </div>
                    </div>
                    <div className='content-card'>
                        <div className="card-header">
                            <h2 className='maintenance'>Maintenance</h2>
                            <button className="collapse-btn" onClick={() => toggleCard('maintenance')}>{expandedCards.maintenance ? '▲' : '▼'}</button>
                        </div>
                        <div className={`card-body ${expandedCards.maintenance ? 'expanded' : 'collapsed'}`}>
                            {properties.map((property) => (
                                <div className='properties' key={property.id}>
                                    <div className='property-info'>
                                        <h4>{property.name}</h4>
                                        <MaintenanceRequests property={property.id} />
                                    </div>
                                    <hr className='divider'></hr>
                                </div>
                            ))}
                        </div>
                    </div>
                    <div className='content-card'>
                        <div className="card-header">
                            <h2 className='documents'>Documents</h2>
                            <button className="collapse-btn" onClick={() => toggleCard('documents')}>{expandedCards.documents ? '▲' : '▼'}</button>
                        </div>
                        <div className={`card-body ${expandedCards.documents ? 'expanded' : 'collapsed'}`}>
                            {properties.map((property) => (
                                <div className='properties' key={property.id}>
                                    <div className='property-info'>
                                        <h4>{property.name}</h4>
                                    </div>
                                    <hr className='divider'></hr>
                                </div>
                            ))}
                        </div>
                    </div>
                    <div className='content-card'>
                        <div className="card-header">
                            <h2>Tenants</h2>
                            <button className="collapse-btn" onClick={() => toggleCard('tenants')}>{expandedCards.tenants ? '▲' : '▼'}</button>
                        </div>
                        <div className={`card-body ${expandedCards.tenants ? 'expanded' : 'collapsed'}`}>
                            {properties.map((property) => (
                                <div className='properties' key={property.id}>
                                    <div className='property-info'>
                                        <h4>{property.name}</h4>
                                        <Tenants property={property.id} />
                                    </div>
                                    <hr className='divider'></hr>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
                   
            </div>
        </div>
    )
}