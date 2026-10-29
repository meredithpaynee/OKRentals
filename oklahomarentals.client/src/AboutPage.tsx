import { Link } from 'react-router-dom'
import SiteLayout, { ContactBand, PageHeader } from './site/SiteLayout'
import { cloudinaryUrl } from './site/images'

export default function AboutPage() {
    return (
        <SiteLayout>
            <PageHeader title="About us" image="stillwater-downtown.jpg" breadcrumbs={[{ to: '/', label: 'Home' }]} />

            <section className="section">
                <div className="container split">
                    <div>
                        <span className="eyebrow">Who we are</span>
                        <h2>Family-run student housing</h2>
                        <p>
                            Oklahoma College Rentals is a family-run rental property company specializing in
                            student housing across Oklahoma, with homes near the University of Oklahoma in
                            Norman and Oklahoma State University in Stillwater.
                        </p>
                        <p>
                            We pride ourselves on high-quality properties, strong customer service, and a
                            genuine personal connection with every tenant. Our goal is to help you find the
                            right home for your needs.
                        </p>
                        <Link className="text-link" to="/contact">Get in touch &rarr;</Link>
                    </div>
                    <img className="split-image" src={cloudinaryUrl('oklahoma-landscape_jblnki.jpg', 1200)} alt="Oklahoma landscape" />
                </div>
            </section>

            <section className="section section--surface">
                <div className="container">
                    <div className="section-heading">
                        <span className="eyebrow">Where we are</span>
                        <h2>Two college towns</h2>
                    </div>
                    <div className="map-grid">
                        <figure>
                            <img src={cloudinaryUrl('norman-map.jpg', 1000)} alt="Map of Norman, Oklahoma" loading="lazy" />
                            <figcaption>
                                <Link to="/properties/Norman">Norman</Link>
                                <span>University of Oklahoma</span>
                            </figcaption>
                        </figure>
                        <figure>
                            <img src={cloudinaryUrl('stillwater-map.jpg', 1000)} alt="Map of Stillwater, Oklahoma" loading="lazy" />
                            <figcaption>
                                <Link to="/properties/Stillwater">Stillwater</Link>
                                <span>Oklahoma State University</span>
                            </figcaption>
                        </figure>
                    </div>
                </div>
            </section>

            <ContactBand />
        </SiteLayout>
    )
}
