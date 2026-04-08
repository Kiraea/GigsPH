<script setup lang="ts">
const { createPost, isMutating } = usePosts()

const title = ref('')
const description = ref('')
const file = ref<File | null>(null)

const handleSubmit = async () => {
  await createPost({ title: title.value, description: description.value, file: file.value })
}
</script>

<template>
  <div class="flex flex-col gap-5 p-5 shadow-md shadow-black rounded-lg">
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