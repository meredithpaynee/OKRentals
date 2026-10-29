// Business details shown across the public site (header, footer, contact page).
export const SITE_NAME = 'Oklahoma College Rentals'
export const CONTACT_EMAIL = 'okcollegerentals@gmail.com'
export const CONTACT_PHONE = '214-202-0008'

export interface Campus {
    slug: string
    name: string
    school: string
    image: string
}

export const CAMPUSES: Campus[] = [
    { slug: 'Norman', name: 'Norman', school: 'University of Oklahoma', image: 'norman-enter_l8zewm.jpg' },
    { slug: 'Stillwater', name: 'Stillwater', school: 'Oklahoma State University', image: 'stillwater-enter_hbatzr.jpg' },
]

export function findCampus(slug: string | undefined): Campus | undefined {
    return CAMPUSES.find(c => c.slug.toLowerCase() === slug?.toLowerCase())
}
