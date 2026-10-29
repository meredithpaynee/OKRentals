import './portal.css'
import { useState, useEffect } from 'react'


/********************************************************************************************************************
 * Get Previous Maintenance Requests by Property
 *******************************************************************************************************************/
interface MaintenanceRequestResponse {
    id: number,
    propertyId: number,
    title: string,
    description: string,
    status: string,
    dateRequested: string,
    dateResolved: string
}

export function MaintenanceRequests({ property }: props) {
    const [requests, setRequests] = useState<MaintenanceRequestResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        fetch('/api/maintenance-requests')
            .then(res => {
                if (!res.ok) throw new Error('Failed to fetch requests')
                return res.json()
            })
            .then((data: MaintenanceRequestResponse[]) => {
                const filtered = property
                    ? data.filter(p => p.propertyId === property)
                    : data;
                setRequests(filtered);
            })
            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, [property]);

    const handleResolve = (id: number) => {
        const dateResolved = new Date().toISOString();

        fetch(`/api/maintenance-requests/${id}/status`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ status: 'Completed' }),
        })
            .then(res => {
                if (!res.ok) throw new Error('Failed to update request')
                setRequests(prev =>
                    prev.map(r =>
                        r.id === id ? { ...r, status: 'Completed', dateResolved } : r
                    )
                );
            })
            .catch(err => setError(err.message));
    };

    if (loading) return <div>Loading...</div>
    if (error) return <div>Error: {error}</div>

    return (
        <div className="admin-content">
            {requests.map((request) => (
                <div key={request.id}>
                    <h4>{request.title}</h4>
                    <span>{request.description}</span>
                    <span> Status: {request.status}</span>
                    {request.status !== 'Resolved' && (
                        <button onClick={() => handleResolve(request.id)}>
                            Mark as Resolved
                        </button>
                    )}
                </div>
            ))}
        </div>
    )
}


interface props {
    property: number;
}


/********************************************************************************************************************
 * Get Previous Payments by Property
 *******************************************************************************************************************/
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

interface TenantPaymentInfo {
    tenantName: string,
    tenantId: number,
    payments: PaymentResponse[]
}

export function PreviousPayments({ property }: props) {
    const [tenantPayments, setTenantPayments] = useState<TenantPaymentInfo[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        Promise.all([
            fetch('/api/payments').then(res => res.json()),
            fetch('/api/tenants').then(res => res.json())
        ])
            .then(([payments, tenants]) => {
                const filtered = property
                    ? payments.filter((p: PaymentResponse) => p.propertyId === property)
                    : payments;

                const grouped = filtered.reduce((acc: Record<number, TenantPaymentInfo>, payment: PaymentResponse) => {
                    const tenant = tenants.find((t: TenantResponse) => t.id === payment.tenantId);
                    if (!acc[payment.tenantId]) {
                        acc[payment.tenantId] = {
                            tenantName: tenant?.name || 'Unknown',
                            tenantId: payment.tenantId,
                            payments: []
                        };
                    }
                    acc[payment.tenantId].payments.push(payment);
                    return acc;
                }, {});

                setTenantPayments(Object.values(grouped));
            })
            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, [property]);

    if (loading) return <div>Loading...</div>
    if (error) return <div>Error: {error}</div>

    return (
        <div className = "admin-content">
            {tenantPayments.map((tenantInfo) => {
                const totalDue = tenantInfo.payments.reduce((sum, p) => sum + p.amountDue, 0);
                const totalPaid = tenantInfo.payments.reduce((sum, p) => sum + p.amountPaid, 0);
                const stillOwes = totalDue - totalPaid;

                return (
                    <div key={tenantInfo.tenantId} style={{ marginBottom: '20px', padding: '10px', border: '1px solid #ddd' }}>
                        <h4>{tenantInfo.tenantName}</h4>
                        <span style={{ color: stillOwes > 0 ? 'red' : 'green' }}>
                            Still Owes: ${stillOwes.toFixed(2)}
                        </span>
                        {tenantInfo.payments.map((payment) => (
                            <div key={payment.id} style={{ marginLeft: '20px', marginBottom: '10px' }}>
                                <span>${payment.amountDue}</span>
                                <span style={{ marginLeft: '10px' }}>
                                    Due: {new Date(payment.dueDate).toLocaleDateString()} |
                                    Paid: ${payment.amountPaid} on {payment.paymentDate ? new Date(payment.paymentDate).toLocaleDateString() : 'Not paid'}
                                </span>
                                <span style={{ marginLeft: '10px' }}>Status: {payment.status}</span>
                            </div>
                        ))}
                    </div>
                );
            })}
        </div>
    );
}



/*****************************************************************************************************************
 * Get tenants by property
*****************************************************************************************************************/
interface TenantResponse {
    id: number,
    propertyId: number,
    name: string,
    email: string,
    phoneNumber: string,
    birthDate: string

}
export function Tenants({ property }: props) {
    const [tenants, setTenants] = useState<TenantResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        fetch('/api/tenants')
            .then(res => {
                if (!res.ok) throw new Error('Failed to fetch requests')
                return res.json()
            })
            .then((data: TenantResponse[]) => {
                const filtered = property
                    ? data.filter(p => p.propertyId === property)
                    : data;
                console.log(filtered);
                setTenants(filtered);
            })

            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, [property]);

    if (loading) return <div>Loading...</div>
    if (error) return <div>Error: {error}</div>



    return (<div className = "admin-content">
        {tenants.map((tenant) => (
            <div key={tenant.id}>
                <h4>{tenant.name}</h4>
                <span>{tenant.phoneNumber}</span>
                <span> - {tenant.email}</span>
            </div>
        ))}
    </div>)
}

/********************************************************************************************************************
 * Create New Property
 *******************************************************************************************************************/


export function CreateProperty() {
    const [statusMessage, setStatusMessage] = useState<string>("");
    const [name, setName] = useState("");
    const [address, setAddress] = useState("");
    const [bedrooms, setBedrooms] = useState(0);
    const [bathrooms, setBathrooms] = useState(0);
    const [campus, setCampus] = useState("");
    const [price, setPrice] = useState(0);
    const [adminId, setAdminId] = useState<number>(1);
    const [nextAvailable, setNextAvailable] = useState("");

    async function handleCreateRequest() {
        setStatusMessage("Creating...");
        try {
            // Because of the proxy in vite.config.ts, we can just hit /api/...
            // The proxy will forward it to https://localhost:7237/api/...
            const response = await fetch('/api/properties', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    Name: name,
                    Campus: campus,
                    Bedrooms: bedrooms,
                    Bathrooms: bathrooms,
                    Price: price,
                    Address: address,
                    AdminId: adminId,
                    NextAvailable: setNextAvailable, 

                }),
            });

            if (response.ok) {
                const data = await response.json();
                setStatusMessage(`Success! Created property ID: ${data.id}`);
                return data;
            } else {
                setStatusMessage(`Error: ${response.status} ${response.statusText}`);
            }
        } catch (error) {
            console.error("Failed to create property", error);
            setStatusMessage("Failed to connect to the server.");
        }
    };

    return (
    <div className='request-card'>
         <p>Property Name:</p>
         <input value={name} onChange={(e) => setName(e.target.value)} />

        <p>Full Address: </p>
         <input onChange={(e) => setAddress(e.target.value)}></input>

        <p>Campus:</p>
         <select value={campus} onChange={(e) => { setCampus(e.target.value); }}>
             <option value="">--select--</option>
             <option>Norman</option>
             <option>Stillwater</option>
         </select>

        <p>Bedrooms:</p>
         <input value={bedrooms} onChange={(e) => setBedrooms(Number(e.target.value))}></input>

        <p>Bathrooms:</p>
         <input value={bathrooms} onChange={(e) => setBathrooms(Number(e.target.value))}></input>

        <p>Total Rent Per Month:</p>
         <input value={price} onChange={(e) => setPrice(Number(e.target.value))}></input>

        <p>Next Available:</p>
            <input type="datetime-local" value={nextAvailable} onChange={() => setNextAvailable(Date())} ></input>

            {/*need to remove, there right now until login gets set up */ }
         <p>Admin ID:</p>
         <input type="number" value={adminId} onChange={(e) => setAdminId(Number(e.target.value))} />

        <button className='request-close' onClick={() => handleCreateRequest()}>Submit</button>
            {statusMessage && <div>{statusMessage}</div>}
    </div>)
}


/********************************************************************************************************************
* Delete Property
*******************************************************************************************************************/

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

export function RemoveProperty({ properties, onDeleted }: { properties: PropertyResponse[], onDeleted?: (id: number) => void }) {
    const [localProps, setLocalProps] = useState<PropertyResponse[]>(properties ?? []);
    const [selectedId, setSelectedId] = useState<number | null>(null);
    const [status, setStatus] = useState<string | null>(null);

    useEffect(() => setLocalProps(properties ?? []), [properties]);

    const handleDelete = async () => {
        if (selectedId == null) return setStatus('Select a property to delete');
        try {
            const res = await fetch(`/api/properties/${selectedId}`, { method: 'DELETE' });
            if (!res.ok) return setStatus(`Delete failed: ${res.status}`);
            setLocalProps(p => p.filter(x => x.id !== selectedId));
            setStatus('Deleted');
            onDeleted?.(selectedId);
            setSelectedId(null);
        } catch (e) {
            setStatus('Delete request failed');
        }
    };

    return (
        <div>
            <h2>Which Property Would you like to delete?</h2>
            <select value={selectedId ?? ''} onChange={(e) => setSelectedId(e.target.value === '' ? null : Number(e.target.value))}>
                <option value="">-- select --</option>
                {localProps.map((property) => (
                    <option key={property.id} value={property.id}>{property.name} — {property.address}</option>
                ))}
            </select>
            <button onClick={handleDelete}>Delete</button>
            {status && <div>{status}</div>}
        </div>
    )
}

/********************************************************************************************************************
* Create Tenant 
*******************************************************************************************************************/

export function CreateTenant() {
    const [statusMessage, setStatusMessage] = useState<string>("");
    const [propertyId, setPropertyId] = useState<number>(1);
    const [phoneNumber, setPhoneNumber] = useState("");
    const [birthDate, setBirthDate] = useState("");
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [properties, setProperties] = useState<PropertyResponse[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetch('/api/properties')
            .then(res => res.json())
            .then((data: PropertyResponse[]) => setProperties(data))
            .catch(err => console.error("Failed to load properties", err))
            .finally(() => setLoading(false));
    }, []);

    async function handleCreateTenant() {
        setStatusMessage("Creating...");
        try {
            const response = await fetch('/api/tenants', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    propertyId: propertyId,
                    phoneNumber: phoneNumber,
                    birthDate: birthDate,
                    name: name,
                    email: email
                    // UserId is auto-created by the backend
                }),
            });

            if (response.ok) {
                const data = await response.json();
                setStatusMessage(`Success! Created tenant ID: ${data.id}`);
                setPropertyId(1);
                setPhoneNumber("");
                setBirthDate("");
                setName("");
                setEmail("");
            } else {
                const errorData = await response.json().catch(() => ({}));
                setStatusMessage(`Error: ${response.status} - ${errorData.message || response.statusText}`);
            }
        } catch (error) {
            console.error("Failed to create tenant", error);
            setStatusMessage("Failed to connect to the server.");
        }
    }

    if (loading) return <div>Loading properties...</div>;

    return (
        <div className='request-card'>
            <p>Select Property:</p>
            <select value={propertyId} onChange={(e) => setPropertyId(Number(e.target.value))}>
                {properties.map((prop) => (
                    <option key={prop.id} value={prop.id}>{prop.name}</option>
                ))}
            </select>

            <p>Tenant Name:</p>
            <input
                value={name}
                onChange={(e) => setName(e.target.value)}
                placeholder="Full name"
            />

            <p>Email Address:</p>
            <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="tenant@example.com"
            />

            <p>Phone Number:</p>
            <input
                type="tel"
                value={phoneNumber}
                onChange={(e) => setPhoneNumber(e.target.value)}
                placeholder="555-1234"
            />

            <p>Birth Date:</p>
            <input
                type="date"
                value={birthDate}
                onChange={(e) => setBirthDate(e.target.value)}
            />

            <button className='request-close' onClick={() => handleCreateTenant()}>Create Tenant</button>
            {statusMessage && <div>{statusMessage}</div>}
        </div>
    );
}

/********************************************************************************************************************
* Remove Tenant
*******************************************************************************************************************/

export function RemoveTenant() {
    const [localTenants, setLocalTenants] = useState<TenantResponse[]>([]);
    const [selectedId, setSelectedId] = useState<number | null>(null);
    const [status, setStatus] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetch('/api/tenants')
            .then(res => {
                if (!res.ok) throw new Error('Failed to fetch tenants');
                return res.json();
            })
            .then((data: TenantResponse[]) => {
                setLocalTenants(data);
            })
            .catch(err => setStatus(`Error loading tenants: ${err.message}`))
            .finally(() => setLoading(false));
    }, []);

    const handleDelete = async () => {
        if (selectedId == null) return setStatus('Select a tenant to delete');
        try {
            const res = await fetch(`/api/tenants/${selectedId}`, { method: 'DELETE' });
            if (!res.ok) return setStatus(`Delete failed: ${res.status}`);
            setLocalTenants(t => t.filter(x => x.id !== selectedId));
            setStatus('Tenant deleted successfully');
            setSelectedId(null);
        } catch (e) {
            setStatus('Delete request failed');
        }
    };

    if (loading) return <div>Loading tenants...</div>;

    return (
        <div>
            <h2>Which Tenant Would you like to delete?</h2>
            <select value={selectedId ?? ''} onChange={(e) => setSelectedId(e.target.value === '' ? null : Number(e.target.value))}>
                <option value="">-- select --</option>
                {localTenants.map((tenant) => (
                    <option key={tenant.id} value={tenant.id}>{tenant.name} — {tenant.phoneNumber}</option>
                ))}
            </select>
            <button onClick={handleDelete}>Delete Tenant</button>
            {status && <div>{status}</div>}
        </div>
    );
}

/********************************************************************************************************************
* Set a Property to Leased (dates as well)
*******************************************************************************************************************/

export function SetPropertyToLeased() {
    const [statusMessage, setStatusMessage] = useState<string>("");
    const [propertyId, setPropertyId] = useState<number>(1);
    const [isLeased, setIsLeased] = useState(true);
    const [properties, setProperties] = useState<PropertyResponse[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetch('/api/properties')
            .then(res => res.json())
            .then((data: PropertyResponse[]) => setProperties(data))
            .catch(err => console.error("Failed to load properties", err))
            .finally(() => setLoading(false));
    }, []);

    async function handleUpdateAvailability() {
        setStatusMessage("Updating...");
        try {
            const response = await fetch(`/api/properties/${propertyId}/availability`, {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    isLeased: isLeased
                }),
            });

            if (response.ok) {
                const data = await response.json();
                setStatusMessage(`Success! Property updated - Leased: ${data.isLeased}`);
            } else {
                const errorData = await response.json().catch(() => ({}));
                setStatusMessage(`Error: ${response.status} - ${errorData.message || response.statusText}`);
            }
        } catch (error) {
            console.error("Failed to update property", error);
            setStatusMessage("Failed to connect to the server.");
        }
    }

    if (loading) return <div>Loading properties...</div>;

    return (
        <div className='request-card'>
            <p>Select Property:</p>
            <select value={propertyId} onChange={(e) => setPropertyId(Number(e.target.value))}>
                {properties.map((prop) => (
                    <option key={prop.id} value={prop.id}>{prop.name}</option>
                ))}
            </select>

            <p>Property Status:</p>
            <select value={isLeased ? 'leased' : 'available'} onChange={(e) => setIsLeased(e.target.value === 'leased')}>
                <option value="available">Available</option>
                <option value="leased">Leased</option>
            </select>

            <button className='request-close' onClick={() => handleUpdateAvailability()}>Update Property</button>
            {statusMessage && <div>{statusMessage}</div>}
        </div>
    );
}

/********************************************************************************************************************
* Record a Payment from a Tenant
*******************************************************************************************************************/

interface RecordPaymentRequest {
    tenantId: number;
    propertyId: number;
    dueDate: string;
    amountDue: number;
    amountPaid: number;
}

export function RecordPayment() {
    const [statusMessage, setStatusMessage] = useState<string>("");
    const [tenantId, setTenantId] = useState<number>(0);
    const [propertyId, setPropertyId] = useState<number>(1);
    const [dueDate, setDueDate] = useState("");
    const [amountDue, setAmountDue] = useState("");
    const [amountPaid, setAmountPaid] = useState("");
    const [allTenants, setAllTenants] = useState<TenantResponse[]>([]);
    const [filteredTenants, setFilteredTenants] = useState<TenantResponse[]>([]);
    const [properties, setProperties] = useState<PropertyResponse[]>([]);
    const [loadingTenants, setLoadingTenants] = useState(true);
    const [loadingProps, setLoadingProps] = useState(true);

    useEffect(() => {
        fetch('/api/tenants')
            .then(res => res.json())
            .then((data: TenantResponse[]) => setAllTenants(data))
            .catch(err => console.error("Failed to load tenants", err))
            .finally(() => setLoadingTenants(false));

        fetch('/api/properties')
            .then(res => res.json())
            .then((data: PropertyResponse[]) => {
                setProperties(data);
                if (data.length > 0) {
                    setPropertyId(data[0].id);
                }
            })
            .catch(err => console.error("Failed to load properties", err))
            .finally(() => setLoadingProps(false));
    }, []);

    // Filter tenants when propertyId changes
    useEffect(() => {
        const filtered = allTenants.filter(t => t.propertyId === propertyId);
        setFilteredTenants(filtered);
        // Always set to first filtered tenant or 0 if none
        if (filtered.length > 0) {
            setTenantId(filtered[0].id);
        } else {
            setTenantId(0);
        }
    }, [propertyId, allTenants]);

    async function handleRecordPayment() {
        if (tenantId === 0) {
            setStatusMessage("Please select a valid tenant");
            return;
        }

        setStatusMessage("Recording...");
        try {
            const response = await fetch('/api/payments', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    tenantId: tenantId,
                    propertyId: propertyId,
                    dueDate: dueDate,
                    paymentDate: new Date().toISOString(),
                    amountDue: parseFloat(amountDue) || 0,
                    amountPaid: parseFloat(amountPaid) || 0,
                    status: "Paid"
                }),
            });

            if (response.ok) {
                const data = await response.json();
                setStatusMessage(`Success! Recorded payment ID: ${data.id}`);
                setDueDate("");
                setAmountDue("");
                setAmountPaid("");
            } else {
                const errorData = await response.json().catch(() => ({}));
                setStatusMessage(`Error: ${response.status} - ${errorData.message || response.statusText}`);
            }
        } catch (error) {
            console.error("Failed to record payment", error);
            setStatusMessage("Failed to connect to the server.");
        }
    }

    if (loadingTenants || loadingProps) return <div>Loading data...</div>;

    return (
        <div className='request-card'>
            <p>Select Property:</p>
            <select value={propertyId} onChange={(e) => setPropertyId(Number(e.target.value))}>
                {properties.map((prop) => (
                    <option key={prop.id} value={prop.id}>{prop.name}</option>
                ))}
            </select>

            <p>Select Tenant:</p>
            <select value={tenantId} onChange={(e) => setTenantId(Number(e.target.value))}>
                {filteredTenants.length > 0 ? (
                    filteredTenants.map((tenant) => (
                        <option key={tenant.id} value={tenant.id}>{tenant.name}</option>
                    ))
                ) : (
                    <option value={0}>No tenants in this property</option>
                )}
            </select>

            <p>Due Date:</p>
            <input
                type="date"
                value={dueDate}
                onChange={(e) => setDueDate(e.target.value)}
            />

            <p>Amount Due:</p>
            <input
                value={amountDue}
                onChange={(e) => setAmountDue(e.target.value)}
                placeholder="0.00"
            />

            <p>Amount Paid:</p>
            <input
                value={amountPaid}
                onChange={(e) => setAmountPaid(e.target.value)}
                placeholder="0.00"
            />

            <button className='request-close' onClick={() => handleRecordPayment()}>Record Payment</button>
            {statusMessage && <div>{statusMessage}</div>}
        </div>
    );
}