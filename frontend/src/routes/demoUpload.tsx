import { createFileRoute } from '@tanstack/react-router'
export const Route = createFileRoute('/demoUpload')({
    component: uploadTestPage,
})

function uploadTestPage() {
    const handleFile = (e) => {
        const file: File = e.target.files?.[0]
        if (!file) {
            console.log('no file')
        }
        console.log(file.type)
        console.log(file.name)
        console.log(file.size)
        console.log(file.lastModified)
        console.log(file.arrayBuffer())
        console.log(file.bytes())
        console.log(file.stream())
    }
    return (
        <div>
            <input type="file" onChange={handleFile}></input>
        </div>
    )
}
