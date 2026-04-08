<script setup lang="ts">
/*

export interface CreatePostRequest {
    title : string,
    description : string,
    file? : File | null
}


 */

const title = ref("");
const description = ref("");
const url = ref("");
</script>

<template>

  <div class="flex flex-col gap-5 p-5 shadow-md shadow-black rounded-lg">
    <div class="flex flex-col">
      <div>Title</div>
      <input type="text">
    </div>

    <div class="flex flex-col">
      <div>Description</div>
      <input type="text"></input>
    </div>


    <div class="flex flex-col">
      <div>Upload File</div>
      <input type="file"></input>
    </div>
  </div>
</template>
