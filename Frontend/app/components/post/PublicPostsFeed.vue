<script setup lang="ts">

import PublicPostCard from "~/components/post/PublicPostCard.vue";
import {FetchError} from 'ofetch';
defineProps<{
  posts: GetPublicPostsResponse[] | null | undefined
  postsPending: boolean,
  isMutating: boolean,
  postsError: FetchError | null | undefined
}>()
</script>

<template>
  <div class="flex flex-col items-center gap-y-10">
    <div v-if="postsPending">...Loading </div>
    
    
    <template v-else-if="posts && posts.length > 0" >
      <PublicPostCard v-for="post in posts"  :post="post" :key="post.Id"/>
    </template>
    
    
    <div v-else-if="postsError">{{postsError.data}}</div>
    <div v-else>No Posts Found</div>
  </div>
</template>

<style scoped>

</style>