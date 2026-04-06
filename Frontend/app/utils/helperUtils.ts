



export const formatDate = (date: Date | string | undefined):string => {

    if (!date){
        return ""
    }
    const newDate = new Date(date);

    return new Intl.DateTimeFormat('en-US', {
        year: "numeric",
        month: "long",
        day: "2-digit"
    }).format(newDate)

}