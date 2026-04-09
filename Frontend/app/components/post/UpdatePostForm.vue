<script setup lang="ts">
const { updatePost, isMutating } = usePostMutations()



const emit = defineEmits<{
  close: []
}>()

const props = defineProps<{
  post: GetPublicPostsResponse
}>()

const title = ref(props.post.title)
const description = ref(props.post.description)
const file = ref<File | null>(null)
const isRemoved = ref(false);

watch(file, newFile => {
  if (newFile != null){
    isRemoved.value = true
  }else{
    
  }
})

const handleIsRemove = () => {
  isRemoved.value = !isRemoved.value
}


const handleSubmit = async () => {
  console.log(title.value,props.post.id)
  await updatePost(props.post.id, {title: title.value, description: description.value, file:file.value, removeMedia:isRemoved.value})
  emit("close");
}
</script>

<template>
  <div class="relative bg-white flex flex-col gap-5 p-5 shadow-md shadow-gray-500 rounded-lg">
    <button class="absolute top-2 right-2" @click="emit('close')" >X</button>
    <div class="flex flex-col">
      <label>Title</label>
      <input v-model="title" type="text" required />
    </div>
    <div class="flex flex-col">
      <label>Description</label>
      <input v-model="description" type="text" required />
    </div>
    <div v-if="post.mediaUrl">
      <label>Remove File?</label>
      <button @click="handleIsRemove" :disabled="file != null">{{isRemoved ? 'Yes' : 'No'}}</button>
    </div> 

    
    <div class="flex flex-col">
      <label>Upload File</label>
      <input type="file" @change="e => file = (e.target as HTMLInputElement).files?.[0] ?? null" />
    </div>
    <button @click="handleSubmit" :disabled="isMutating">
      {{ isMutating ? 'Posting...' : 'Update Post' }}
    </button>
  </div>
</template>
