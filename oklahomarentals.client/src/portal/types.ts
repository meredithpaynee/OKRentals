// Shapes returned by the API, as used by the admin portal.

export interface PropertyResponse {
    id: number
    name: string
    campus: string
    bedrooms: number
    bathrooms: number
    price: number
    address: string
    isLeased: boolean
}

export interface TenantResponse {
    id: number
    propertyId: number
    name: string
    email: string
    phoneNumber: string
    birthDate: string
}

export interface PaymentResponse {
    id: number
    tenantId: number
    propertyId: number
    dueDate: string
    paymentDate: string | null
    amountDue: number
    amountPaid: number
    status: string
}

export interface MaintenanceRequestResponse {
    id: number
    propertyId: number
    title: string
    description: string
    status: string
    dateRequested: string
    dateResolved: string | null
}
