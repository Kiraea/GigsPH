<script setup lang="ts">
import Header from "~/components/layout/Header.vue";
import ErrorBox from "~/components/layout/ErrorBox.vue";
import FriendBox from "~/components/layout/FriendBox.vue";
</script>



<template>
  <div class="flex flex-col min-h-screen bg-white">

    <Header/>
    <div class="flex flex-row flex-1">
      <main class="flex-1 ">
        <slot/>
      </main>
      <aside class="w-[15%] sticky top-16 h-[calc(100vh-64px)] overflow-y-auto">
        <slot name="sidebar">
          <FriendBox></FriendBox>
        </slot>
      </aside>
    </div>

    <ErrorBox/>

  </div>
  
</template>