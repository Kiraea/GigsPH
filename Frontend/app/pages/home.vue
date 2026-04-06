<script setup lang="ts">

import {usePosts} from "~/composables/usePosts";
import FriendBox from "~/components/layout/FriendBox.vue";
definePageMeta({
  layout:"default",
  
})
const {posts, postsPending, postsError, isMutating, createPost } = usePosts()

// item start so that if theres 1k post the friendbox wouldnt stretch
</script>

<template>
  
  <div class="flex-1 flex flex-row items-start">
    <div class="w-[85%] bg-gray-100 min-h-[calc(100vh-64px)]">
      <PostPublicPostsFeed :posts="posts" :posts-pending="postsPending" :isMutating="isMutating" :postsError="postsError" />
    </div>
    <div class="w-[15%] sticky top-16 h-[calc(100vh-64px)] overflow-y-auto">
      <FriendBox/>
    </div>
  </div>
</template>
