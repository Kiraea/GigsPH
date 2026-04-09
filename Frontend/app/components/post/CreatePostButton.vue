<script setup lang="ts">
import CreatePostForm from "~/components/post/CreatePostForm.vue";

const isOpen = ref(false);

</script>

<template>
<div>
  <button @click="isOpen= true" >Create Post</button>
  <Teleport to="body">
    <div v-if="isOpen" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <CreatePostForm @close="isOpen = false"/>
    </div>
  </Teleport>
</div>
</template><style scoped>

</style>