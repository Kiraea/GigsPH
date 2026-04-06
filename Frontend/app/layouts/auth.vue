<script setup lang="ts">

import ErrorBox from "~/components/layout/ErrorBox.vue";
</script>

<template>
  <div class="flex flex-col items-center justify-center min-h-screen  bg-white ">
      <slot/>
      <ErrorBox></ErrorBox>
  </div>
</template>
