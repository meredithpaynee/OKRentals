import * as React from 'react';
import './portal.css'
import Header from "./Header"
import NavPortal from "./NavPortal"
import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'



const formatDate = (s?: string) => {
    if (!s) return 'TBD';
    const d = new Date(s);
    if (isNaN(d.getTime()) || d.getFullYear() < 1900) return 'TBD';
    return d.toLocaleDateString(undefined, { year: 'numeric', month: 'long', day: 'numeric' });
};

interface MaintenanceRequestResponse {
    id: number,
    propertyId: number,
    title: string,
    description: string,
    status: string,
    dateRequested: string,
    dateResolved: string
}
interface Props {
    access: string;
    address: string;
}

export default function PortalHome(props: Props) {
    const navigate = useNavigate();
    const [showRequest, setShowRequest] = useState(false);

    const [statusMessage, setStatusMessage] = useState<string>("");
    const [title, setTitle] = useState("")
    const [description, setDescription] = useState("")
    const [urgency, setUrgency] = useState("")

    async function handleCreateRequest() {
        setStatusMessage("Creating...");
        try {
            // Because of the proxy in vite.config.ts, we can just hit /api/...
            // The proxy will forward it to https://localhost:7237/api/...
            const response = await fetch('/api/maintenance-requests', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    TenantId: 123,
                    PropertyId: 1,
                    Title: title,
                    Description: description,
                    RequestPriority: urgency
                }),
            });

            if (response.ok) {
                const data = await response.json();
                setStatusMessage(`Success! Created maintenance ID: ${data.id}`);
                return data;
            } else {
                setStatusMessage(`Error: ${response.status} ${response.statusText}`);
            }
        } catch (error) {
            console.error("Failed to create request", error);
            setStatusMessage("Failed to connect to the server.");
        }
    };

    const id = 1;
    const [requests, setRequests] = useState<MaintenanceRequestResponse[]>([]);
    const [pending, setPending] = useState<MaintenanceRequestResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        fetch('/api/maintenance-requests?propertyId=1')
            .then(res => { if (!res.ok) throw new Error('Failed to fetch requests'); return res.json(); })
            .then((data: MaintenanceRequestResponse[]) => {
                const filtered = id
                    ? data.filter(p => p.propertyId === id)
                    : data;
                const formatted = filtered.map(r => ({
                    ...r,
                    dateRequested: formatDate(r.dateRequested),
                    dateResolved: formatDate(r.dateResolved),
                }));
                setRequests(formatted);
                const onlyPending = pending ? formatted.filter(p => p.status === "Pending") : formatted;
                setPending(onlyPending);
            })
            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, [id]);

    if (loading) return <div>Loading...</div>
    if (error) return <div>Error: {error}</div>

    return (
        <div className='portal'>
            <Header />
            <NavPortal />
            <div className='top'>
                <h1>{props.access} Portal</h1>
                <h3>{props.address}</h3>
            </div>
            <div className='information'>
                <div className='overview'>
                    <div className='overview-card'>
                        <h3>Current Balance</h3>
                        <p className='balance'></p>
                        <p className='due-date'></p>
                    </div>
                    <div className='overview-card'>
                        <h3>Active Maintenance Requests</h3>
                        {pending.map((request) => (
                            <div key={request.id}>
                                <h4>{request.title}</h4>
                                <p>{request.description}</p>
                                <span>Created {request.dateRequested} - </span>
                                <span> Status: {request.status} ({request.dateResolved})</span>
                            </div>
                        ))}
                        <a className='see-all' href='#maintenance'>See All</a>
                    </div>
                </div>

            </div>
            <div className='lower'>
                <div className='left-column'>
                    <div className='content-card'><h2>Payment History</h2>
                        {/*call from database all previous payments
                        <button onClick={() => navigate(`/payment`)} >Make Payment</button>*/}
                        <PreviousPayments />
                    </div>
                    <div className='content-card'><h2 className='maintenance'>Maintenance</h2>
                        {/*call previous maintenance requests*/}
                        <button onClick={() => setShowRequest(true)} >Make Request</button>
                        {showRequest && <div><div className="modal-backdrop" />
                            <div className="modal-popup request-card">
                            <h2>Make Request</h2>
                            <p>Title: </p>
                            <input onChange={(e) => setTitle(e.target.value)}></input>
                            <p>Description:</p>
                            <input onChange={(e) => setDescription(e.target.value)}></input>
                            <p>Urgency:</p>
                                <input onChange={(e) => setUrgency(e.target.value)}></input>
                            <div className = "request-buttons">
                            <button className='request-close' onClick={()=>setShowRequest(false) }>Cancel</button>
                            <button className='request-close' onClick={() => handleCreateRequest()}>Submit</button>
                            </div>
                           </div> 
                        </div>}
                        <div>
                            {requests.map((request) => (
                                <div key={request.id}>
                                    <h4>{request.title}</h4>
                                    <p>{request.description}</p>
                                    <span>Created {request.dateRequested} - </span>
                                    <span> Status: {request.status} ({request.dateResolved})</span>
                                </div>
                            ))}
                        </div>
                    </div>
                    <div className='content-card'><h2 className='documents'>Documents</h2>
                        {/*display any documents*/}
                    </div>
                </div>
            </div>
        </div>
    )
}


interface PaymentResponse {
    id: number,
    tenantId: number,
    propertyId: number,
    dueDate: string,
    paymentDate: string,
    amountDue: number,
    amountPaid: number,
    status: string
}

function PreviousPayments() {
    const id = 1;
    const [payments, setPayments] = useState<PaymentResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const formatDate = (s?: string) => {
        if (!s) return 'TBD';
        const d = new Date(s);
        if (isNaN(d.getTime()) || d.getFullYear() < 1900) return 'TBD';
        return d.toLocaleDateString(undefined, { year: 'numeric', month: 'long', day: 'numeric' });
    };

    useEffect(() => {
        fetch('/api/payments?TenantID=123')
            .then(res => { if (!res.ok) throw new Error('Failed to fetch requests'); return res.json(); })
            .then((data: PaymentResponse[]) => {
                const formatted = data.map(p => ({
                    ...p,
                    dueDate: formatDate(p.dueDate),
                    paymentDate: formatDate(p.paymentDate),
                }));
                setPayments(formatted);
            })
            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, [id]);

    if (loading) return <div>Loading...</div>
    if (error) return <div>Error: {error}</div>

    return (<div>
        {payments.map((payment) => (

            <div key={payment.id}>
                <h2>${payment.amountDue}</h2>
                <span>Due: {payment.dueDate} | Paid: ${payment.amountPaid} on {payment.paymentDate}</span>
                <span> Status: {payment.status}</span>
            </div>
        ))}
    </div>)
}