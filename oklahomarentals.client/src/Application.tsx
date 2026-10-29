import * as React from 'react';
import { useState, useEffect } from 'react'
import { Cloudinary } from '@cloudinary/url-gen';
import "./apply.css"
import Header from './Header'

interface PropertyResponse {
    id: number
    name: string
    campus: string
    bedrooms: number
    bathrooms: number
    price: number
    address: string
    isLeased: boolean
    nextAvailable: string
}

export default function Application() {
    const [application, setApplicationVisible] = useState(false);
    const cld = new Cloudinary({ cloud: { cloudName: 'djt05lgzv' } });

    const img = cld
        .image('Norman_020_h7cj50.jpg')
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

    const [properties, setProperties] = useState<PropertyResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

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

    if (loading) return <div>Loading...</div>
    if (error) return <div>Error: {error}</div>


    return (
        <div className="application-section">
            <Header />
            <div className="hero-section" style={heroStyle}>
                <div className="hero-overlay"></div>
                <div className="hero-content">
                    <h1 className="hero-title">Apply to Rent</h1>
                </div>
            </div> 

            {!application && <button className = "apply-button" onClick={() => setApplicationVisible(true)}>Application</button>}
            {application &&
            <div className = 'application'>
                <div className="card"><h2>Personal Information</h2>
                    <h4>Name:</h4>
                    <input id="applicant-name"></input>
                    <h4>Email:</h4>
                    <input id="applicant-email"></input>
                    <h4>Phone Number: </h4>
                    <input id="applicant-number"></input>
                    <h4>Date Of Birth: </h4>
                    <input type='date' id="applicant-dob"></input>
                    <h4>Driver's License:</h4>
                    <input id="applicant-license"></input>
                    <h4>Social Security Number:</h4>
                    <input id="applicant-ssn"></input>
                </div>

                <div className="card"><h2>Academic Information</h2>
                    <h4>College / University Name:</h4>
                    <input id="applicant-college"></input>
                    <h4>Field of Study / Major:</h4>
                    <input id="applicant-major"></input>
                    <h4>Expected Graduation Date: </h4>
                    <input type='date' id="applicant-graduation"></input>
                    <h4>Driver's License:</h4>
                    <input id="applicant-license"></input>
                    <h4>Social Security Number:</h4>
                    <input id="applicant-ssn"></input>
                </div>

                <div className = "card"><h2>Property Information</h2>
                    <h4>Property:</h4>
                    <select >
                        {properties.map((property) => {
                            return (
                                <option key={property.id}>{property.name}</option>
                            );
                        })}
                    </select>
                    <h4>Requested Lease Start Date: </h4>
                    <input type='date' id="start-date"></input>
                    <h4>Lease Term: </h4>
                    <input id="lease-term"></input>
                </div>

                <div className="card"><h2>Previous Residence History (Past 2 years)</h2>
                    <h3>Current Residence</h3>
                    <h4>Address:</h4>
                    <input id="applicant-current-address"></input>
                    <h4>Landlord / Property Manager Name: </h4>
                    <input id="applicant-landlord"></input>
                    <h4>Phone / Email: </h4>
                    <input id="applicant-landlord"></input>
                    <h4>Dates of Occupancy:</h4>
                    <label for="applicant-for">From</label>
                    <input type='date' id="applicant-for"></input>
                    <label for="applicant-to">To</label>
                    <input type='date' id="applicant-to"></input>
                    <h4>Reason for Leaving:</h4>
                    <input id="applicant-rfl"></input>
                    <hr/>
                    <h3>Previous Residence (if applicable)</h3>
                    <h4>Address:</h4>
                    <input id="applicant-current-address"></input>
                    <h4>Landlord / Property Manager Name: </h4>
                    <input id="applicant-landlord"></input>
                    <h4>Phone / Email: </h4>
                    <input id="applicant-landlord"></input>
                    <h4>Dates of Occupancy:</h4>
                    <label for="applicant-for">From</label>
                    <input type='date' id="applicant-for"></input>
                    <label for="applicant-to">To</label>
                    <input type='date' id="applicant-to"></input>
                    <h4>Reason for Leaving:</h4>
                    <input id="applicant-rfl"></input>
                </div>

                <div className="card"><h2>Background</h2>
                    <h3>If yes, provide an explanation below.</h3>
                    <h3>Have you ever: </h3>
                    <form>
                        <h4>Been evicted?</h4>
                        <input type="radio" id="evicted-yes" name="evicted" value="Yes"></input>
                        <label for="evicted-yes">Yes</label>
                        <input type="radio" id="evicted-no" name="evicted" value="no"></input>
                        <label for="evicted-no">No</label>
                    </form>
                    <form>
                        <h4>Broken a lease?</h4>
                        <input type="radio" id="broken-yes" name="broken" value="Yes"></input>
                        <label for="evicted-yes">Yes</label>
                        <input type="radio" id="broken-no" name="broken" value="No"></input>
                        <label for="filed-no">No</label>
                    </form>
                    <form>
                        <h4>Filed for bankruptcy?</h4>
                        <input type="radio" id="filed-yes" name="filed" value="Yes"></input>
                        <label for="filed-yes">Yes</label>
                        <input type="radio" id="filed-no" name="filed" value="No"></input>
                        <label for="filed-no">No</label>
                    </form>
                    <form>
                        <h4>Been convicted of a felony or misdemeanor (excluding minor traffic violations)?</h4>
                        <input type="radio" id="convicted-yes" name="convicted" value="Yes"></input>
                        <label for="convicted-yes">Yes</label>
                        <input type="radio" id="convicted-no" name="convicted" value="no"></input>
                        <label for="convicted-no">No</label>
                    </form>
                    <h4>Explanation (if applicable):</h4>
                    <input id="explanation"></input>
                </div>
                <button>Submit</button>
                </div>}
        </div>
    )
}