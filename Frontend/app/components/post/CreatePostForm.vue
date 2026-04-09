<script setup lang="ts">
const { createPost, isMutating } = usePostMutations()

const title = ref('')
const description = ref('')
const file = ref<File | null>(null)

const emit = defineEmits<{
  close: []
}>()

const handleSubmit = async () => {
  await createPost({ title: title.value, description: description.value, file: file.value })
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
    <div class="flex flex-col">
      <label>Upload File</label>
      <input type="file" @change="e => file = (e.target as HTMLInputElement).files?.[0] ?? null" />
    </div>
    <button @click="handleSubmit" :disabled="isMutating">
      {{ isMutating ? 'Posting...' : 'Create Post' }}
    </button>
  </div>
</template>
