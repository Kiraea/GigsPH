<script setup lang="ts">
import Header from "~/components/layout/Header.vue";
import ErrorBox from "~/components/layout/ErrorBox.vue";
</script>
<template>
  <div class="flex flex-col min-h-screen bg-white">
    <Header/>
    <main class="flex-1 flex flex-col">
      <slot/>
    </main>
    <ErrorBox></ErrorBox>
  </div>
  
</template>