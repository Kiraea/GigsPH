export function ConvertDateTimeToReadable(dateInput: string): string {
    //  ISO 8601 Timestamp
    //2026-11-22T03:00:00Z

    const newDate = new Date(dateInput)

    if (Number.isNaN(newDate.getTime())) {
        return 'Date Unavailable'
    }

    return newDate
        .toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: 'numeric',
            minute: 'numeric',
            timeZone: 'UTC',
        })
        .toString()
}
