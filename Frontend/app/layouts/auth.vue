<script setup lang="ts">

import ErrorBox from "~/components/layout/ErrorBox.vue";
</script>

<template>
  <div class="flex flex-col items-center justify-center min-h-screen bg-no-repeat bg-[url('mainBg.jpg')] bg-cover bg-blend-multiply bg-black/80">
      <slot/>
      <ErrorBox></ErrorBox>
  </div>
</template>
