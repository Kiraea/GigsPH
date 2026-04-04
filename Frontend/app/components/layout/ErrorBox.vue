<script setup lang="ts">
 const {errors} =  useErrorHandler();
 
 
 // IN TEMPLATE IT AUTO UNWRAPS
</script>


<template>
  
<div v-if="errors" class="bg-black">
  <div v-for="error in errors" :key="error.id">
    {{error.message}}
  </div>
</div>
</template>

<style scoped>

</style>