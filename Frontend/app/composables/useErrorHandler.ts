

interface AppError {
    id: string,
    message: string,
}
const ParseErrors = (err: any):string[] => {
    if (typeof err == "string"){
        return [err]
    }
    if (Array.isArray(err)){
        return err.map(e => e.toString());
    }
    if (err && typeof err === "object"){
        const {data, message} = err as Record<string,any>;
        
        if (data){
            return Array.isArray(data) ? data.map(d => d.toString()) : [data]
        }
        if (message) return [message];
    }
    return ["Something W3nt Wrong"]
}
export const useErrorHandler =() => {
    const errors = useState<AppError[]| null>('global-error', ()=>null);
    
    const removeError = (id: string) => {
        if (!errors.value){
            return
        }
        errors.value = errors.value.filter(err => err.id !== id);
        if (errors.value.length ===0 ) errors.value =null;
    }
    const setErrors = (newErrors : any) => {
        if (!newErrors) return
        
        const messages = ParseErrors(newErrors)
        
        messages.forEach((message,i) => {
           const id = crypto.randomUUID();
            errors.value = [
                ...(errors.value ?? []), {id: id, message: message}
            ]
            setTimeout(()=> {
               removeError(id) 
            }, 5000)
        })
        

    }
    return {setErrors,  errors}
}