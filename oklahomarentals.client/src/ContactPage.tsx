import { Link } from 'react-router-dom'
import SiteLayout, { PageHeader } from './site/SiteLayout'

interface props {
    email: string;
    phone: string;
}

export default function ContactPage(props: props) {
    return (
        <SiteLayout>
            <PageHeader
                title="Contact us"
                intro="Questions about a property, a showing, or your application? We're happy to help."
                image="norman-downtown_ys47pm.jpg"
                breadcrumbs={[{ to: '/', label: 'Home' }]}
            />

            <section className="section">
                <div className="container split">
                    <div>
                        <h2>Reach us directly</h2>
                        <ul className="contact-list">
                            <li><span>Phone</span><a href={`tel:${props.phone}`}>{props.phone}</a></li>
                            <li><span>Email</span><a href={`mailto:${props.email}`}>{props.email}</a></li>
                            <li><span>Locations</span><span>Norman and Stillwater, Oklahoma</span></li>
                        </ul>
                    </div>
                    <div className="stack">
                        <div className="panel">
                            <h3>Ready to apply?</h3>
                            <p>One application covers any of our properties.</p>
                            <Link className="button" to="/apply">Start an application</Link>
                        </div>
                        <div className="panel">
                            <h3>Current residents</h3>
                            <p>
                                Submit maintenance requests and view your payment history in the resident portal.
                            </p>
                        </div>
                    </div>
                </div>
            </section>
        </SiteLayout>
    )
}
