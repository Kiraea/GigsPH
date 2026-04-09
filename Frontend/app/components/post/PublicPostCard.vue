<script setup lang="ts">


import {formatDate} from "~/utils/helperUtils";
import UpdatePostForm from "~/components/post/UpdatePostForm.vue";
defineProps<{
  post: GetPublicPostsResponse
}>()



const isOpen = ref(false);


</script>

<template>
  <div class="bg-white rounded-xl shadow-sm border border-gray-100 flex flex-col gap-y-2 w-[40%]">




    <div>
      <button @click="isOpen= true" >Update Post</button>
      <Teleport to="body">
        <div v-if="isOpen" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <UpdatePostForm :post="post" @close="isOpen = false"/>
        </div>
      </Teleport>
    </div>
    
    
    <!-- Header: avatar + name + title -->
    <div class="flex flex-col px-5">
      <div class="flex items-center gap-3">
        <div>
          <div class="flex flex-row items-center gap-5">
            <NuxtLink :to="`/profile/${post.userId}`" class="w-12 h-12 rounded-full bg-red-500 text-lg text-black font-bold items-center flex justify-center">{{post.displayName.charAt(0)}}</NuxtLink>
            <div class="font-semibold text-gray-900 text-md">{{ post.displayName }}</div>
          </div>
          <div class="text-xs text-gray-400">{{formatDate(post.createdAt)}}</div>
        </div>
      </div>
    </div>

      <!-- Title + description -->
    <div class="flex flex-col px-5">
      <div class="font-semibold text-gray-800 mb-1">{{ post.title }}</div>
      <div class="text-gray-600 text-md">{{ post.description }}</div>
    </div>
   
    <!-- Video -->
    <video v-if="post.mediaUrl" controls class="w-full max-h-96 object-cover bg-black">
      <source :src="post.mediaUrl" />
    </video>
    
    
    <div class="flex flex-row justify-between">
      <div>
        <Icon name="uil:heart" class="bg-red-600 text-red-700 size-7"/>
        <Icon name="mdi:heart" class="bg-red-600 text-red-700 size-7"/>  
      </div>
      <label class="font-semibold">654 Likes</label>
      
      
    </div>

  </div>
</template>