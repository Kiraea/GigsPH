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

export function ConvertDateTimeToReadableSummary(dateInput: string): string {
    //  ISO 8601 Timestamp
    //2026-11-22T03:00:00Z

    const newDate = new Date(dateInput)

    if (Number.isNaN(newDate.getTime())) {
        return 'Date Unavailable'
    }

    const currentDate = new Date()

    const timeDifference = currentDate.getTime() - currentDate.getTime()

    const oneDayMiliseconds = 1000 * 60 * 60 * 24

    // this means that its less than 1 day so we use hour:min
    if (timeDifference - oneDayMiliseconds < 0) {
        return newDate.toLocaleTimeString('en-US', {
            hour: 'numeric',
            minute: 'numeric',
        })
    } else {
        return newDate.toLocaleDateString('en-US', {
            day: 'numeric',
            month: 'short',
        })
    }
}
