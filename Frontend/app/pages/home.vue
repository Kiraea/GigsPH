<script setup lang="ts">

import {usePosts} from "~/composables/usePosts";
import FriendBox from "~/components/layout/FriendBox.vue";
import CreatePostButton from "~/components/post/CreatePostButton.vue";

const {posts, postsPending, postsError} = usePosts()
const {isMutating, createPost, updatePost, deletePost } = usePostMutations()

// item start so that if theres 1k post the friendbox wouldnt stretch
</script>

<template>
  <NuxtLayout name="normal">
      <CreatePostButton/>
      <PostPublicPostsFeed :posts="posts" :posts-pending="postsPending" :isMutating="isMutating" :postsError="postsError" />
  </NuxtLayout>
</template>
