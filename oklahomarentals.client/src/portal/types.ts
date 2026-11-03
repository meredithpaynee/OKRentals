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
    description: string
    nextAvailable: string
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

export interface ResidenceResponse {
    address: string | null
    landlord: string | null
    landlordContact: string | null
    from: string | null
    to: string | null
    reasonForLeaving: string | null
}

export interface ApplicationResponse {
    id: number
    propertyId: number
    propertyName: string | null
    status: string
    name: string
    email: string
    phoneNumber: string
    birthDate: string
    moveInDate: string
    dateSubmitted: string
    driversLicense: string | null
    college: string | null
    major: string | null
    expectedGraduation: string | null
    leaseTerm: string | null
    currentResidence: ResidenceResponse | null
    previousResidence: ResidenceResponse | null
    evicted: boolean | null
    brokenLease: boolean | null
    bankruptcy: boolean | null
    convicted: boolean | null
    explanation: string | null
}
