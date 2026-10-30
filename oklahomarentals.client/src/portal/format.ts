// Formatting helpers shared by the resident and admin portals.

export function formatMoney(amount: number): string {
    return amount.toLocaleString('en-US', { style: 'currency', currency: 'USD' })
}

// "Mar 1, 2026", or an em dash for missing / placeholder dates.
export function formatDate(value?: string | null): string {
    if (!value) return '—'
    const d = new Date(value)
    if (isNaN(d.getTime()) || d.getFullYear() < 1900) return '—'
    return d.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' })
}

export function isOpenRequest(status: string): boolean {
    return !['completed', 'resolved', 'cancelled'].includes(status.toLowerCase())
}
