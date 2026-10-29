import { useEffect, useState } from 'react'

export interface PropertyResponse {
    id: number
    name: string
    campus: string
    bedrooms: number
    bathrooms: number
    price: number
    address: string
    description: string
    isLeased: boolean
    nextAvailable: string
}

export interface CloudinaryImage {
    publicId: string
    secureUrl: string
}

interface Loadable<T> {
    data: T
    loading: boolean
    error: string | null
}

export function useProperties(): Loadable<PropertyResponse[]> {
    const [state, setState] = useState<Loadable<PropertyResponse[]>>({ data: [], loading: true, error: null })

    useEffect(() => {
        fetch('/api/properties')
            .then(res => {
                if (!res.ok) throw new Error('Failed to fetch properties')
                return res.json()
            })
            .then((data: PropertyResponse[]) => setState({ data, loading: false, error: null }))
            .catch(err => setState({ data: [], loading: false, error: err.message }))
    }, [])

    return state
}

export function fetchImages(propertyName: string): Promise<CloudinaryImage[]> {
    return fetch(`/api/properties/${encodeURIComponent(propertyName)}/images`).then(res => {
        if (!res.ok) throw new Error('Failed to fetch images')
        return res.json()
    })
}

export function formatRent(price: number): string {
    return price.toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 })
}

function parseAvailableDate(value: string): Date | null {
    if (!value) return null
    const d = new Date(value)
    return isNaN(d.getTime()) || d.getFullYear() < 1900 ? null : d
}

// e.g. "Available now", "Available August 1, 2026", "Leased"
export function availabilityLabel(property: PropertyResponse): string {
    const date = parseAvailableDate(property.nextAvailable)
    const formatted = date?.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })

    if (property.isLeased) return formatted ? `Leased · available ${formatted}` : 'Leased'
    if (date && date > new Date()) return `Available ${formatted}`
    return 'Available now'
}
